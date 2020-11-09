using Google.Cloud.Firestore;

namespace GCloud.ResourceAccess.Domain.Entities
{
	[FirestoreData]
	public class UserEntity : IEntity
	{
		#region User Members

		public string Id { get; set; }

		[FirestoreProperty]
		public string First
		{
			get; set;
		}

		[FirestoreProperty]
		public string Last
		{
			get; set;
		}

		[FirestoreProperty]
		public int Year
		{
			get; set;
		}

		[FirestoreProperty]
		public int Hits { get; set; }

		#endregion User Members

	}
}