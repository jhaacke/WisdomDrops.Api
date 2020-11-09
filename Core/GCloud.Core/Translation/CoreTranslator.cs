using GCloud.Core.Contract;
using GCloud.ResourceAccess.Excel.Contract;

using Domain = GCloud.ResourceAccess.Domain.Contract;

namespace GCloud.Core.Translation
{
	public static class CoreTranslator
	{
		#region Members

		public static Affirmation TranslateToAffirmation( Domain.Affirmation source )
		{
			return new Affirmation
			{
				Id = source.Id,
				CategoryName = source.CategoryName,
				Description = source.Description,
				More = source.More,
				Topic = source.Topic,
				Weight = source.Weight
			};
		}

		public static Domain.Affirmation TranslateToDomainAffirmation( WisdomDrop source )
		{
			return new Domain.Affirmation
			{
				Id = source.Id?.ToString(),
				CategoryName = source.Category,
				Description = source.Tip,
				More = source.More,
				Weight = source.Rating
			};
		}

		public static Submission TranslateToSubmission( Domain.Submission source )
		{
			return new Submission
			{
				Id = source.Id,
				Affirmation = TranslateToAffirmation( source.Affirmation ),
				Subscriber = TranslateToSubscriber( source.Subscriber ),
				SubmissionTime = source.SubmissionTime
			};
		}

		public static Subscriber TranslateToSubscriber( Domain.Subscriber source )
		{
			return new Subscriber
			{
				Id = source.Id,
				TimeWindowEnd = source.TimeWindowEnd,
				TimeWindowStart = source.TimeWindowStart,
				FirstName = source.FirstName,
				Phone = source.Phone
			};
		}

		#endregion
	}
}