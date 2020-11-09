using System;

namespace GCloud.Core.Contract
{
	public class Subscriber
	{
		#region Properties

		public string FirstName { get; set; }

		public string Id { get; set; }

		public string Phone { get; set; }

		public DateTime TimeWindowEnd { get; set; }

		public DateTime TimeWindowStart { get; set; }

		public string Username { get; set; }

		#endregion
	}
}