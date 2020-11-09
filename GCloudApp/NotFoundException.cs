using System;

namespace GCloudApp
{
	public class NotFoundException : ApplicationException
	{
		#region Constructors

		public NotFoundException()
		{
		}

		public NotFoundException( string message )
			: base( message )
		{
		}

		#endregion
	}
}