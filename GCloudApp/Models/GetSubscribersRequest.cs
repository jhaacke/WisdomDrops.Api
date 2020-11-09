namespace GCloudApp.Models
{
	public class GetSubscribersRequest : GetRequestBase
	{
		#region Properties

		public bool? IncludeCount { get; set; }

		#endregion
	}
}