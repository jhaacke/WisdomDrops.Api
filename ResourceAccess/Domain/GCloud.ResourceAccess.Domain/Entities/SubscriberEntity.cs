using System;

using Google.Cloud.Firestore;

namespace GCloud.ResourceAccess.Domain.Entities
{
	[FirestoreData]
	public class SubscriberEntity : IEntity
	{
		#region Properties

		[FirestoreProperty]
		public string FirstName { get; set; }

		public string Id { get; set; }

		[FirestoreProperty]
		public string Phone { get; set; }

		[FirestoreProperty]
		public DateTime TimeWindowEnd { get; set; }

		[FirestoreProperty]
		public DateTime TimeWindowStart { get; set; }

		[FirestoreProperty]
		public string Username { get; set; }

		#endregion
	}
}