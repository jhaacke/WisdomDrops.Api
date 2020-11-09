using GCloud.Utility.Sms.Contract;

using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace GCloud.Utility.Sms
{
	public class TwilioSmsAdapter : ISmsAdapter
	{
		#region Fields

		private readonly string mAccountId;

		private readonly string mAuthToken;

		private readonly string mSenderPhoneNumber;

		#endregion

		#region Constructors

		public TwilioSmsAdapter( string accountId, string authToken, string senderPhoneNumber )
		{
			mAccountId = accountId;
			mAuthToken = authToken;
			mSenderPhoneNumber = senderPhoneNumber;
		}

		#endregion

		#region Members

		public void Send( string phoneNumber, string message )
		{
			TwilioClient.Init( mAccountId, mAuthToken );
			MessageResource.Create( body: message, from: new Twilio.Types.PhoneNumber( mSenderPhoneNumber ), to: new Twilio.Types.PhoneNumber( phoneNumber ) );
		}

		#endregion
	}
}