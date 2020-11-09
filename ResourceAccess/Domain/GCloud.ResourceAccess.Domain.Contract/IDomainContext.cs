using System.Collections.Generic;
using System.Threading.Tasks;

namespace GCloud.ResourceAccess.Domain.Contract
{
	public interface IDomainContext
	{
		#region Members

		Task<string> AddAffirmation( Affirmation affirmation );

		Task<string> AddCategory( Category category );

		Task AddSubmission( Submission submission );

		Task<string> AddTopic( Topic topic );

		Task<bool> DeleteAffirmation( string affirmationId );

		Task<bool> DeleteCategory( string categoryId );

		Task<bool> DeleteTopic( string topicId );

		Task<IEnumerable<Affirmation>> GetAffirmations();

		Task<IEnumerable<Category>> GetCategories();

		Task<Category> GetCategoryByName( string categoryName );

		Task<Subscriber> GetSubscriberById( string id );

		Task<Subscriber> GetSubscriberByUsername( string username );

		Task<IEnumerable<Subscriber>> GetSubscribers();

		Task<Topic> GetTopicByName( string topicName );

		Task<IEnumerable<Topic>> GetTopics();

		Task<bool> UpdateAffirmation( Affirmation affirmation );

		#endregion
	}
}