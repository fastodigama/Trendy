using Trendy.Models;

namespace Trendy.Interfaces
{
    public interface ICommentService
    {
        /// <summary>
        /// Adds a new comment to a topic.
        /// </summary>
        /// <param name="createCommentDto">DTO containing comment details.</param>
        /// <returns>A ServiceResponse indicating success or failure.</returns>
        Task<ServiceResponse> AddComment(CreateCommentDto createCommentDto);

        /// <summary>
        /// Lists all comments for a specific topic.
        /// </summary>
        /// <param name="topicId">The ID of the topic.</param>
        /// <returns>A list of CommentDto objects.</returns>
        Task<IEnumerable<CommentDto>> ListCommentsByTopic(int topicId);

        /// <summary>
        /// Deletes a comment by its ID.
        /// </summary>
        /// <param name="commentId">The ID of the comment to delete.</param>
        /// <returns>A ServiceResponse indicating success or failure.</returns>
        Task<ServiceResponse> DeleteComment(int commentId);

        /// <summary>
        /// Retrieves all comments for a specific topic.
        /// </summary>
        /// <param name="topicId">The ID of the topic.</param>
        /// <returns>A list of CommentDto objects associated with the topic.</returns>
        Task<IEnumerable<CommentDto>> GetCommentsForTopic(int topicId);

        /// <summary>
        /// Updates an existing comment.
        /// </summary>
        /// <param name="commentId">The ID of the comment to update.</param>
        /// <param name="newText">The new comment text.</param>
        /// <returns>A ServiceResponse indicating success or failure.</returns>
        Task<ServiceResponse> UpdateComment(int commentId, string newText);


    }
}
