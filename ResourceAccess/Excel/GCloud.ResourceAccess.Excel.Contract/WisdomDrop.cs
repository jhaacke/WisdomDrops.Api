using System;

namespace GCloud.ResourceAccess.Excel.Contract
{
	public class WisdomDrop
	{
		#region Properties

		public string Category { get; set; }

		public Guid? Id { get; set; }

		public string KeyTopic { get; set; }

		public string More { get; set; }

		public int Rating { get; set; }

		public string Tip { get; set; }

		#endregion
	}
}