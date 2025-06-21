using Trendy.Models;

namespace Trendy.Interfaces
{
    public interface ITopicService
    {

        /// <summary>
        /// Retrieves a list of topics.
        /// </summary>
        /// <returns>
        /// A collection of TopicDto objects representing available topics.
        /// </returns>
        Task<IEnumerable<TopicDto>> ListTopics();

        /// <summary>
        /// Retrieves a topic by its id.
        /// </summary>
        /// <param name="topicId">The ID of the topic to find.</param>
        /// <returns>
        /// A ServiceResponse object containing the status and messages.
        /// </returns>
        /// <remarks>
        /// If a topic is found, the response status will be 'Found'.
        /// If the topic does not exist, the response status will be 'NotFound'.
        /// </remarks>

        Task<ServiceResponse> GetTopicById(int topicId);


        /// <summary>
        /// Retrieves a list of all topics within a given category.
        /// </summary>
        /// <param name="CategoryId">The ID of the category used to filter topics.</param>
        /// <returns>
        /// A ServiceResponse object containing the status, messages, and a list of topics.
        /// </returns>
        /// <remarks>
        /// If topics are found, the response status will be 'Found'.
        /// If the category does not exist or has no topics, the response status will be 'NotFound'.
        /// </remarks>

        Task<ServiceResponse> ListTopicsByCategoryId(int categoryId);

        /// <summary>
        /// Adds a new topic to the database.
        /// </summary>
        /// <param name="createTopicDto">The DTO containing topic details, such as title and description.</param>
        /// <param name="userId">The ID of the currently logged-in user (must be an admin).</param>
        /// <returns>
        /// A ServiceResponse indicating success ('Created') or failure ('Error').
        /// </returns>
        /// <remarks>
        /// If successful, the response will contain the newly created topic ID.
        /// If an error occurs, the response will return error messages.
        /// </remarks>
        Task<ServiceResponse> AddNewTopic(CreateTopicDto createTopicDto, string userId);

        /// <summary>
        /// Updates an existing topic's title, description, and linked categories.
        /// </summary>
        /// <param name="updateTopicDto">The DTO containing the topic ID, updated title, description, and new category links.</param>
        /// <returns>
        /// A ServiceResponse indicating success ('Updated') or failure ('NotFound' or 'Error').
        /// </returns>
        /// <remarks>
        /// If the topic ID does not exist, the response will return a 'NotFound' status.
        /// If the update is successful, the response will contain a success message and the updated topic ID.
        /// </remarks>
        Task<ServiceResponse> UpdateTopic(UpdateTopicDto updateTopicDto);

        /// <summary>
        /// Deletes a topic by its ID, including all associated category links.
        /// </summary>
        /// <param name="topicId">The ID of the topic to delete.</param>
        /// <returns>
        /// A ServiceResponse indicating deletion status ('Deleted', 'NotFound', or 'Error').
        /// </returns>
        Task<ServiceResponse> DeleteTopic(int topicId);

        /// <summary>
        /// Searches topics by a given keyword found in the title or description.
        /// </summary>
        /// <param name="keyword">The keyword to search for within topic titles and descriptions.</param>
        /// <returns>
        /// A task that returns a list of TopicDto objects matching the keyword.
        /// </returns>
        Task<IEnumerable<TopicDto>> SearchTopicsByKeyword(string keyword);


    }
}
