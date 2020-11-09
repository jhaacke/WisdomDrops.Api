namespace GCloudApp.Models
{
	public class UploadFromSpreadsheetResponse
	{
		#region Properties

		public string ErrorMessage { get; set; }

		public string Report { get; set; }

		public bool Success { get; set; }

		#endregion
	}
}