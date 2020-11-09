namespace GCloudApp.Models
{
	/// <summary>
	/// Common request criteria fields.
	/// </summary>
	public abstract class GetRequestBase
	{
		#region Properties

		/// <summary>
		/// Indicates page number of data to retrieve.
		/// </summary>
		public int? PageNumber { get; set; }

		/// <summary>
		/// Indicates the page size of data to retrieve.
		/// </summary>
		public int? PageSize { get; set; }

		/// <summary>
		/// Text to use to filter results.
		/// </summary>
		public string SearchText { get; set; }

		/// <summary>
		/// Sort direction: 1=ASC, 2=DESC
		/// </summary>
		public int? SortDir { get; set; }

		/// <summary>
		/// Field to use to sort the retrieved users.
		/// </summary>
		public string SortField { get; set; }

		#endregion
	}
}