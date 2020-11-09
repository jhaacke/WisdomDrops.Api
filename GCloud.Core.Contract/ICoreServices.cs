using System.Collections.Generic;
using System.Threading.Tasks;

namespace GCloud.Core.Contract
{
	public interface ICoreServices
	{
		#region Members

		Task<IEnumerable<Affirmation>> GetAllAffirmations();

		Task<IEnumerable<Subscriber>> GetAllSubscribers();

		Task ScheduleAffirmationsForDay();

		Task SendAffirmationToSubscriber( string id );

		Task<AffirmationImportResult> ImportAffirmationsFromSpreadsheet( string base64Data );

		#endregion
	}
}