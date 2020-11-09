using Google.Cloud.Firestore;

namespace GCloud.ResourceAccess.Domain.Entities
{
	[FirestoreData]
	public class AffirmationEntity : IEntity
	{
		#region Properties

		[FirestoreProperty]
		public string CategoryName { get; set; }

		[FirestoreProperty]
		public string Description { get; set; }

		public string Id { get; set; }

		[FirestoreProperty]
		public string More { get; set; }

		[FirestoreProperty]
		public string Topic { get; set; }

		[FirestoreProperty]
		public int Weight { get; set; }

		#endregion
	}
}