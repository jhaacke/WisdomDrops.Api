using Google.Cloud.Firestore;

namespace GCloud.ResourceAccess.Domain.Entities
{
	[FirestoreData]
	public class TopicEntity : IEntity
	{
		#region Properties

		public string Id { get; set; }

		[FirestoreProperty]
		public string Name { get; set; }

		#endregion
	}
}