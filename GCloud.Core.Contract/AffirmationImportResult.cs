using System;

namespace GCloud.Core.Contract
{
	public class AffirmationImportResult
	{
		#region Properties

		public Tuple<int, int> AffirmationsAdded { get; set; }

		public Tuple<int, int> AffirmationsDeleted { get; set; }

		public Tuple<int, int> AffirmationsUpdated { get; set; }

		public Tuple<int, int> CategoriesAdded { get; set; }

		public Tuple<int, int> CategoriesDeleted { get; set; }

		public Tuple<int, int> TopicsAdded { get; set; }

		public Tuple<int, int> TopicsDeleted { get; set; }

		#endregion
	}
}