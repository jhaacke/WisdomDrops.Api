using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using GCloud.ResourceAccess.Domain.Contract;
using GCloud.ResourceAccess.Domain.Entities;
using GCloud.ResourceAccess.Domain.Translation;

using Google.Cloud.Firestore;

namespace GCloud.ResourceAccess.Domain
{
	public class DomainContext : IDomainContext
	{
		#region Fields

		private readonly string mGoogleProjectId;

		private FirestoreDb mFirestoreDb;

		#endregion

		#region Constructors

		public DomainContext( string googleProjectId )
		{
			mGoogleProjectId = googleProjectId;
		}

		#endregion

		#region Properties

		protected FirestoreDb Database => mFirestoreDb ?? ( mFirestoreDb = _GetDatabase() );

		#endregion

		#region Members

		public async Task<string> AddAffirmation( Affirmation affirmation )
		{
			var entity = DomainTranslator.TranslateToAffirmationEntity( affirmation );
			var result = await _AddEntity( entity, "Affirmations" );
			return result;
		}

		public async Task<string> AddCategory( Category category )
		{
			var entity = DomainTranslator.TranslateToCategoryEntity( category );
			var result = await _AddEntity( entity, "Categories" );
			return result;
		}

		public async Task AddSubmission( Submission submission )
		{
			var entity = DomainTranslator.TranslateToSubmissionEntity( submission );
			await _AddEntity( entity, "Submissions" );
		}

		public async Task<string> AddTopic( Topic topic )
		{
			var entity = DomainTranslator.TranslateToTopicEntity( topic );
			var result = await _AddEntity( entity, "Topics" );
			return result;
		}

		public async Task<bool> DeleteAffirmation( string affirmationId )
		{
			var result = await _DeleteById( "Affirmations", affirmationId );
			return result;
		}

		public async Task<bool> DeleteCategory( string categoryId )
		{
			var result = await _DeleteById( "Categories", categoryId );
			return result;
		}

		public async Task<bool> DeleteTopic( string topicId )
		{
			var result = await _DeleteById( "Topics", topicId );
			return result;
		}

		public async Task<IEnumerable<Affirmation>> GetAffirmations()
		{
			var entities = await _GetAll<AffirmationEntity>( "Affirmations" );
			var result = entities.Select( DomainTranslator.TranslateToAffirmation ).ToArray();
			return result;
		}

		public async Task<IEnumerable<Category>> GetCategories()
		{
			var entities = await _GetAll<CategoryEntity>( "Categories" );
			var result = entities.Select( DomainTranslator.TranslateToCategory ).ToArray();
			return result;
		}

		public async Task<Category> GetCategoryByName( string categoryName )
		{
			var entity = await _GetByValue<CategoryEntity>( "Categories", "Name", categoryName.Trim() );
			var result = entity != null
				? DomainTranslator.TranslateToCategory( entity )
				: null;
			return result;
		}

		public async Task<Subscriber> GetSubscriberById( string id )
		{
			var entity = await _GetById<SubscriberEntity>( "Subscribers", id );
			var result = entity != null
				? DomainTranslator.TranslateToSubscriber( entity )
				: null;
			return result;
		}

		public async Task<Subscriber> GetSubscriberByUsername( string username )
		{
			var entity = await _GetByValue<SubscriberEntity>( "Subscribers", "Username", username );
			var result = entity != null
				? DomainTranslator.TranslateToSubscriber( entity )
				: null;
			return result;
		}

		public async Task<IEnumerable<Subscriber>> GetSubscribers()
		{
			var entities = await _GetAll<SubscriberEntity>( "Subscribers" );
			var result = entities.Select( DomainTranslator.TranslateToSubscriber ).ToArray();
			return result;
		}

		public async Task<Topic> GetTopicByName( string topicName )
		{
			var entity = await _GetByValue<TopicEntity>( "Topics", "Name", topicName.Trim() );
			var result = entity != null
				? DomainTranslator.TranslateToTopic( entity )
				: null;
			return result;
		}

		public async Task<IEnumerable<Topic>> GetTopics()
		{
			var entities = await _GetAll<TopicEntity>( "Topics" );
			var result = entities.Select( DomainTranslator.TranslateToTopic ).ToArray();
			return result;
		}

		public async Task<bool> UpdateAffirmation( Affirmation affirmation )
		{
			var entity = DomainTranslator.TranslateToAffirmationEntity( affirmation );
			var result = await _UpdateEntity( "Affirmations", entity );
			return result;
		}

		public async Task<bool> CategoryExists( string categoryName )
		{
			var entity = await _GetByValue<CategoryEntity>( "Categories", "Name", categoryName.Trim() );
			return entity != null;
		}

		public async Task IncrementUser( string userId, string fieldName, int incrementValue )
		{
			var docRef = Database.Collection( "users" ).Document( userId );
			await docRef.UpdateAsync( fieldName, FieldValue.Increment( incrementValue ) );
		}

		public async Task UpdateUser( string userId, UserEntity user )
		{
			var dict = user.ToDictionary();
			var docRef = Database.Collection( "users" ).Document( userId );
			await docRef.UpdateAsync( dict );
		}

		private async Task<string> _AddEntity<T>( T entity, string collectionName )
			where T : IEntity
		{
			string result = null;

			try
			{
				DocumentReference docRef = await Database.Collection( collectionName ).AddAsync( entity );
				result = docRef.Id;
			}
			catch( Exception )
			{
				// ignored
			}

			return result;
		}

		private async Task<bool> _DeleteById( string collectionName, string id )
		{
			var result = true;

			try
			{
				await Database.Collection( collectionName ).Document( id ).DeleteAsync( Precondition.MustExist );
			}
			catch( Exception )
			{
				result = false;
			}

			return result;
		}

		private async Task<IEnumerable<T>> _GetAll<T>( string collectionName )
			where T : IEntity
		{
			var query = Database.Collection( collectionName );
			var snapshot = await query.GetSnapshotAsync();
			var result = snapshot.Select( _ToEntity<T> );
			return result;
		}

		private async Task<T> _GetById<T>( string collectionName, string id )
			where T : IEntity
		{
			var result = default( T );
			var query = Database.Collection( collectionName ).Document( id );
			var snapshot = await query.GetSnapshotAsync();
			if( snapshot.Exists )
			{
				result = _ToEntity<T>( snapshot );
			}

			return result;
		}

		private async Task<T> _GetByValue<T>( string collectionName, string fieldName, object fieldValue )
			where T : IEntity
		{
			var result = default( T );
			var query = Database.Collection( collectionName ).WhereEqualTo( fieldName, fieldValue );
			var snapshot = await query.GetSnapshotAsync();
			if( snapshot.Documents.Count > 0 )
			{
				if( snapshot.Documents.Count > 1 )
				{
					throw new ApplicationException( $"Multiple documents found with {fieldName} equal to {fieldValue}." );
				}
				else
				{
					result = _ToEntity<T>( snapshot.Documents.First() );
				}
			}

			return result;
		}

		private FirestoreDb _GetDatabase()
		{
			var result = FirestoreDb.Create( mGoogleProjectId );
			return result;
		}

		private T _ToEntity<T>( DocumentSnapshot snapshot )
			where T : IEntity
		{
			var result = snapshot.ConvertTo<T>();
			result.Id = snapshot.Id;
			return result;
		}

		private async Task<bool> _UpdateEntity( string collectionName, IEntity entity )
		{
			var result = true;

			try
			{
				//			 	await Database.Collection( collectionName ).Document( entity.Id ).SetAsync( entity );
				var dict = entity.ToDictionary();
				var docRef = Database.Collection( collectionName ).Document( entity.Id );
				await docRef.UpdateAsync( dict, Precondition.MustExist );
			}
			catch( Exception )
			{
				result = false;
			}

			return result;
		}

		#endregion
	}
}