
namespace GCloud.ResourceAccess.Domain.Contract
{
	public class User
	{
		#region User Members

		public string Id { get; set; }

		public string First
		{
			get; set;
		}

		public string Last
		{
			get; set;
		}

		public int Year
		{
			get; set;
		}

		public int Hits { get; set; }

		#endregion User Members

	}
}