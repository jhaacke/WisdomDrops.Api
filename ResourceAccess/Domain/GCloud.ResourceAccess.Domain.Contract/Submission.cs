﻿using System;

namespace GCloud.ResourceAccess.Domain.Contract
{
	public class Submission
	{
		#region Properties

		public Affirmation Affirmation { get; set; }

		public string Id { get; set; }

		public DateTime SubmissionTime { get; set; }

		public Subscriber Subscriber { get; set; }

		#endregion
	}
}