using Api = GCloudApp.Models;
using Core = GCloud.Core.Contract;

namespace GCloudApp.Translation
{
	public static class ApiTranslator
	{
		#region Members

		public static Api.Affirmation TranslateToApiAffirmation( Core.Affirmation affirmation )
		{
			return new Api.Affirmation
			{
				Id = affirmation.Id,
				CategoryName = affirmation.CategoryName,
				Description = affirmation.Description,
				More = affirmation.More,
				Topic = affirmation.Topic,
				Weight = affirmation.Weight
			};
		}

		public static Api.Submission TranslateToApiSubmission( Core.Submission submission )
		{
			return new Api.Submission
			{
				Id = submission.Id,
				Affirmation = TranslateToApiAffirmation( submission.Affirmation ),
				Subscriber = TranslateToApiSubscriber( submission.Subscriber ),
				SubmissionTime = submission.SubmissionTime
			};
		}

		public static Api.Subscriber TranslateToApiSubscriber( Core.Subscriber subscriber )
		{
			return new Api.Subscriber
			{
				Id = subscriber.Id,
				TimeWindowEnd = subscriber.TimeWindowEnd,
				TimeWindowStart = subscriber.TimeWindowStart,
				Username = subscriber.Username,
				FirstName = subscriber.FirstName,
				Phone = subscriber.Phone
			};
		}

		#endregion
	}
}