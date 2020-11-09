using System;

using Google.Cloud.Firestore;

namespace GCloud.ResourceAccess.Domain.Entities
{
	[FirestoreData]
	public class SubmissionEntity : IEntity
	{
		#region Properties

		[FirestoreProperty]
		public AffirmationEntity Affirmation { get; set; }

		[FirestoreProperty]
		public string Id { get; set; }

		[FirestoreProperty]
		public DateTime SubmissionTime { get; set; }

		[FirestoreProperty]
		public SubscriberEntity Subscriber { get; set; }

		#endregion
	}
}