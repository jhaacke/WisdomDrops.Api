namespace GCloud.Utility.Sms.Contract
{
	public interface ISmsAdapter
	{
		#region Members

		void Send( string phoneNumber, string message );

		#endregion
	}
}