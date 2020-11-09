using GCloud.ResourceAccess.Domain.Contract;
using GCloud.ResourceAccess.Domain.Entities;

namespace GCloud.ResourceAccess.Domain.Translation
{
	public static class DomainTranslator
	{
		#region Members

		public static Affirmation TranslateToAffirmation( AffirmationEntity entity )
		{
			return new Affirmation
			{
				Id = entity.Id,
				CategoryName = entity.CategoryName,
				Description = entity.Description,
				More = entity.More,
				Topic = entity.Topic,
				Weight = entity.Weight
			};
		}

		public static AffirmationEntity TranslateToAffirmationEntity( Affirmation affirmation )
		{
			return new AffirmationEntity
			{
				Id = affirmation.Id,
				CategoryName = affirmation.CategoryName,
				Description = affirmation.Description,
				More = affirmation.More,
				Topic = affirmation.Topic,
				Weight = affirmation.Weight
			};
		}

		public static Category TranslateToCategory( CategoryEntity entity )
		{
			return new Category
			{
				Id = entity.Id,
				Name = entity.Name
			};
		}

		public static CategoryEntity TranslateToCategoryEntity( Category source )
		{
			return new CategoryEntity
			{
				Id = source.Id,
				Name = source.Name
			};
		}

		public static Submission TranslateToSubmission( SubmissionEntity entity )
		{
			return new Submission
			{
				Affirmation = TranslateToAffirmation( entity.Affirmation ),
				Id = entity.Id,
				Subscriber = TranslateToSubscriber( entity.Subscriber )
			};
		}

		public static SubmissionEntity TranslateToSubmissionEntity( Submission submission )
		{
			return new SubmissionEntity
			{
				Affirmation = TranslateToAffirmationEntity( submission.Affirmation ),
				Id = submission.Id,
				SubmissionTime = submission.SubmissionTime,
				Subscriber = TranslateToSubscriberEntity( submission.Subscriber )
			};
		}

		public static Subscriber TranslateToSubscriber( SubscriberEntity subscriber )
		{
			return new Subscriber
			{
				Id = subscriber.Id,
				TimeWindowEnd = subscriber.TimeWindowEnd,
				TimeWindowStart = subscriber.TimeWindowStart,
				FirstName = subscriber.FirstName,
				Phone = subscriber.Phone
			};
		}

		public static SubscriberEntity TranslateToSubscriberEntity( Subscriber subscriber )
		{
			return new SubscriberEntity
			{
				Id = subscriber.Id,
				FirstName = subscriber.FirstName,
				Phone = subscriber.Phone,
				TimeWindowEnd = subscriber.TimeWindowEnd,
				TimeWindowStart = subscriber.TimeWindowStart,
			};
		}

		public static Topic TranslateToTopic( TopicEntity entity )
		{
			return new Topic
			{
				Id = entity.Id,
				Name = entity.Name
			};
		}

		public static TopicEntity TranslateToTopicEntity( Topic source )
		{
			return new TopicEntity
			{
				Id = source.Id,
				Name = source.Name
			};
		}

		#endregion
	}
}