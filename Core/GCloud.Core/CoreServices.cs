using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using GCloud.Core.Contract;
using GCloud.Core.Translation;
using GCloud.ResourceAccess.Excel.Contract;
using GCloud.Utility.Sms.Contract;

using Domain = GCloud.ResourceAccess.Domain.Contract;

namespace GCloud.Core
{
	public class CoreServices : ICoreServices
	{
		#region Fields

		private readonly Domain.IDomainContext mDomainContext;

		private readonly IExcelAdapter mExcelAdapter;

		private readonly ISmsAdapter mSmsAdapter;

		#endregion

		#region Constructors

		public CoreServices( Domain.IDomainContext domainContext, IExcelAdapter excelAdapter, ISmsAdapter smsAdapter )
		{
			mDomainContext = domainContext;
			mExcelAdapter = excelAdapter;
			mSmsAdapter = smsAdapter;
		}

		#endregion

		#region Members

		public async Task<IEnumerable<Affirmation>> GetAllAffirmations()
		{
			var affirmations = await mDomainContext.GetAffirmations();
			var result = affirmations.Select( CoreTranslator.TranslateToAffirmation ).ToArray();
			return result;
		}

		public async Task<IEnumerable<Subscriber>> GetAllSubscribers()
		{
			var subscribers = await mDomainContext.GetSubscribers();
			var result = subscribers.Select( CoreTranslator.TranslateToSubscriber ).ToArray();
			return result;
		}

		public async Task<AffirmationImportResult> ImportAffirmationsFromSpreadsheet( string base64Data )
		{
			var wisdomDropsFromSpreadsheet = mExcelAdapter.ExtractWisdomDropsFromExcel( base64Data );
			var categorySyncResult = await _SyncCategories( wisdomDropsFromSpreadsheet );
			var topicSyncResult = await _SyncTopics( wisdomDropsFromSpreadsheet );
			var affirmationSyncResult = await _SyncAffirmations( wisdomDropsFromSpreadsheet );

			return new AffirmationImportResult
			{
				AffirmationsAdded = affirmationSyncResult.AddResults,
				AffirmationsDeleted = affirmationSyncResult.DeleteResults,
				AffirmationsUpdated = affirmationSyncResult.UpdateResults,
				CategoriesAdded = categorySyncResult.AddResults,
				CategoriesDeleted = categorySyncResult.DeleteResults,
				TopicsAdded = topicSyncResult.AddResults,
				TopicsDeleted = topicSyncResult.DeleteResults
			};
		}

		private async Task<SynchronizationResult> _SyncAffirmations( WisdomDrop[] drops )
		{
			var affirmations = (await mDomainContext.GetAffirmations()).ToArray();
			var newAffirmations = drops.Where( wisdomDrop => wisdomDrop.Id == null || affirmations.All( affirmation => affirmation.Id != wisdomDrop.Id.Value.ToString() ) ).ToArray();
			var removedAffirmations = affirmations.Where( affirmation => drops.All( drop => !drop.Id.HasValue || !drop.Id.ToString().Equals( affirmation.Id ) ) ).ToArray();
			var updatedAffirmations = drops.Where( drop => drop.Id.HasValue )
				.Select( drop => new
				{
					WisdomDrop = drop,
					Affirmation = affirmations.FirstOrDefault( affirmation => affirmation.Id.Equals( drop.Id.ToString() ) )
				} ).Where( grouping => grouping.Affirmation != null )
				.Select( grouping => _CopyAffirmationFieldsForUpdate( grouping.Affirmation, grouping.WisdomDrop ) )
				.ToArray();

			var deletedCount = await _DeleteAffirmations( removedAffirmations );
			var addedCount = await _AddAffirmations( newAffirmations );
			var updatedCount = await _UpdateAffirmations( updatedAffirmations );

			return new SynchronizationResult
			{
				AddResults = new Tuple<int, int>( newAffirmations.Length, addedCount ),
				DeleteResults = new Tuple<int, int>( removedAffirmations.Length, deletedCount ),
				UpdateResults = new Tuple<int, int>( updatedAffirmations.Length, updatedCount )
			};
		}

		private Domain.Affirmation _CopyAffirmationFieldsForUpdate( Domain.Affirmation affirmation, WisdomDrop drop )
		{
			affirmation.Description = drop.Tip;
			affirmation.CategoryName = drop.Category;
			affirmation.More = drop.More;
			affirmation.Weight = drop.Rating;
			return affirmation;
		}

		private async Task<SynchronizationResult> _SyncCategories( WisdomDrop[] wisdomDrops )
		{
			var categories = ( await mDomainContext.GetCategories() ).ToArray();
			var distinctCategoryNamesToImport = wisdomDrops.Select( drop => drop.Category ).Distinct().ToArray();
			if( distinctCategoryNamesToImport.Any( string.IsNullOrEmpty ) )
			{
				throw new ApplicationException( "Affirmation(s) encountered that are missing a category" );
			}

			var categoryNamesToAdd = distinctCategoryNamesToImport.Where( catToImport => categories.All( cat => !cat.Name.Equals( catToImport, StringComparison.CurrentCultureIgnoreCase ) ) ).ToArray();
			var categoriesToDelete = categories.Where( cat => distinctCategoryNamesToImport.All( catToImport => !catToImport.Equals( cat.Name, StringComparison.CurrentCultureIgnoreCase ) ) ).ToArray();

			var addedCount = await _AddCategories( categoryNamesToAdd );
			var deletedCount = await _DeleteCategories( categoriesToDelete );

			return new SynchronizationResult
			{
				AddResults = new Tuple<int, int>( categoryNamesToAdd.Length, addedCount ),
				DeleteResults = new Tuple<int, int>( categoriesToDelete.Length, deletedCount )
			};
		}

		private async Task<SynchronizationResult> _SyncTopics( WisdomDrop[] wisdomDrops )
		{
			var topics = ( await mDomainContext.GetTopics() ).ToArray();
			var distinctTopicNamesToImport = wisdomDrops.Select( drop => drop.KeyTopic ).Distinct().Where( topic => !string.IsNullOrEmpty( topic ) ).ToArray();
			var topicNamesToAdd = distinctTopicNamesToImport.Where( topicToImport => topics.All( topic => !topic.Name.Equals( topicToImport, StringComparison.CurrentCultureIgnoreCase ) ) ).ToArray();
			var topicsToDelete = topics.Where( topic => distinctTopicNamesToImport.All( topicToImport => !topicToImport.Equals( topic.Name, StringComparison.CurrentCultureIgnoreCase ) ) ).ToArray();

			var addedCount = await _AddTopics( topicNamesToAdd );
			var deletedCount = await _DeleteTopics( topicsToDelete );

			return new SynchronizationResult
			{
				AddResults = new Tuple<int, int>( topicNamesToAdd.Length, addedCount ),
				DeleteResults = new Tuple<int, int>( topicsToDelete.Length, deletedCount )
			};
		}

		private async Task<int> _AddCategories( string[] categoryNamesToAdd )
		{
			var categories = categoryNamesToAdd.Select( name => new Domain.Category
			{
				Name = name
			} ).ToArray();

			var addTasks = categories.Select( category => mDomainContext.AddCategory( category ) );
			var addResults = await Task.WhenAll( addTasks );
			var result = addResults.Count( addResult => !string.IsNullOrEmpty( addResult ) );
			return result;
		}

		private async Task<int> _AddTopics( string[] topicNamesToAdd )
		{
			var topics = topicNamesToAdd.Select( name => new Domain.Topic
			{
				Name = name
			} ).ToArray();

			var addTasks = topics.Select( topic => mDomainContext.AddTopic( topic ) );
			var addResults = await Task.WhenAll( addTasks );
			var result = addResults.Count( addResult => !string.IsNullOrEmpty( addResult ) );
			return result;
		}

		private async Task<int> _DeleteCategories( Domain.Category[] categories )
		{
			var deleteTasks = categories.Select( cat => mDomainContext.DeleteCategory( cat.Id ) );
			var deleteResults = await Task.WhenAll( deleteTasks );
			var result = deleteResults.Count( deleteResult => deleteResult );
			return result;
		}

		private async Task<int> _DeleteTopics( Domain.Topic[] topics )
		{
			var deleteTasks = topics.Select( topic => mDomainContext.DeleteTopic( topic.Id ) );
			var deleteResults = await Task.WhenAll( deleteTasks );
			var result = deleteResults.Count( deleteResult => deleteResult );
			return result;
		}

		private async Task<int> _UpdateAffirmations( Domain.Affirmation[] affirmations )
		{
			var updateTasks = affirmations.Select( affirmation => mDomainContext.UpdateAffirmation( affirmation ) );
			var updateResults = await Task.WhenAll( updateTasks );
			var result = updateResults.Count( updateResult => updateResult );
			return result;
		}

		private async Task<int> _AddAffirmations( WisdomDrop[] wisdomDrops )
		{
			var affirmations = wisdomDrops.Select( CoreTranslator.TranslateToDomainAffirmation ).ToArray();
			var addTasks = affirmations.Select( affirmation => mDomainContext.AddAffirmation( affirmation ) );
			var addResults = await Task.WhenAll( addTasks );
			var result = addResults.Count( addResult => !string.IsNullOrEmpty( addResult ) );
			return result;
		}

		private async Task<int> _DeleteAffirmations( Domain.Affirmation[] affirmations )
		{
			var deleteTasks = affirmations.Select( affirmation => mDomainContext.DeleteAffirmation( affirmation.Id ) );
			var deleteResults = await Task.WhenAll( deleteTasks );
			var result = deleteResults.Count( deleteResult => deleteResult );
			return result;
		}

		public async Task ScheduleAffirmationsForDay()
		{
			var subscribers = ( await mDomainContext.GetSubscribers() ).ToArray();
			var affirmations = ( await mDomainContext.GetAffirmations() ).ToArray();
			var submissionTasks = subscribers.Select( subscriber => _CreateSubmission( subscriber, affirmations ) );
			var submissions = ( await Task.WhenAll( submissionTasks ) ).ToList();
			submissions.ForEach( submission => Task.Factory.StartNew( async () =>
			{
				var utcNow = DateTime.Now.ToUniversalTime();
				var delayTime = submission.SubmissionTime.Subtract( utcNow );
//               var delayTime = new TimeSpan(0, 0, 0, 5 );

//                await Task.Delay( delayTime );

                if( submission.Subscriber.Phone.Equals("+14066908412") )
                {
                    await Task.Delay( delayTime );
                    mSmsAdapter.Send( submission.Subscriber.Phone, _FormatMessage( submission.Subscriber.FirstName, submission.Affirmation.Description ) );
				}

			}) );
		}

		public async Task SendAffirmationToSubscriber( string id )
		{
			var subscriber = await mDomainContext.GetSubscriberById( id );
			if( subscriber != null )
			{
				var affirmations = ( await mDomainContext.GetAffirmations() ).ToArray();
				var randomAffirmation = _SelectRandom( affirmations );
				mSmsAdapter.Send( subscriber.Phone, _FormatMessage( subscriber.FirstName, randomAffirmation.Description ) );
			}
		}

		private async Task<Submission> _CreateSubmission( Domain.Subscriber subscriber, Domain.Affirmation[] affirmations )
		{
			var submission = new Domain.Submission
			{
				Subscriber = subscriber,
				Affirmation = _SelectRandom( affirmations ),
				SubmissionTime = _GetRandomSubmissionDelayForSubscriber( subscriber ).ToUniversalTime()
			};

			await mDomainContext.AddSubmission( submission );
			var result = Translation.CoreTranslator.TranslateToSubmission( submission );
			return result;
		}

		private string _FormatMessage( string name, string affirmationMessage )
		{
			return$"{affirmationMessage}";
		}

		private DateTime _GetRandomSubmissionDelayForSubscriber( Domain.Subscriber subscriber )
		{
			var localStartTime = subscriber.TimeWindowStart.ToLocalTime();
			var localEndTime = subscriber.TimeWindowEnd.ToLocalTime();
			var submitDateTime = DateTime.Today.Date.Add( localStartTime.TimeOfDay );
			if( submitDateTime < DateTime.Now )
			{
				submitDateTime = submitDateTime.AddDays( 1 );
			}

			var startTimeSpan = localStartTime.TimeOfDay;
			var endTimeSpan = localEndTime.TimeOfDay;
			if( endTimeSpan < startTimeSpan )
			{
				endTimeSpan = endTimeSpan.Add( new TimeSpan( 1, 0, 0, 0 ) );
			}

			var timeDiff = endTimeSpan.Subtract( startTimeSpan );
			var randomSeconds = new Random().Next( (int)timeDiff.TotalSeconds );
			var result = submitDateTime.AddSeconds( randomSeconds );
			return result;
		}

		private T _SelectRandom<T>( T[] elements )
		{
			var idx = new Random().Next( elements.Length );
			var result = elements[idx];
			return result;
		}

		#endregion

		private class SynchronizationResult
		{
			public Tuple<int, int> AddResults { get; set; }

			public Tuple<int, int> UpdateResults { get; set; }

			public Tuple<int, int> DeleteResults { get; set; }
		}
	}
}