using Trendy.Interfaces;
using Trendy.Models;
using Microsoft.EntityFrameworkCore;


namespace Trendy.Services
{
    public class CommentService : ICommentService
    {
        private readonly TrendyDbContext _context;

        public CommentService(TrendyDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse> AddComment(CreateCommentDto createCommentDto)
        {
            var response = new ServiceResponse();

            var topic = await _context.Topics.FindAsync(createCommentDto.TopicId);
            if (topic == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Topic not found.");
                return response;
            }

            var comment = new Comment
            {
                CommentText = createCommentDto.CommentText,
                CreatedAt = DateTime.Now,
                TopicId = createCommentDto.TopicId,
                Topic = topic,
                UserId = null // optional for now
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            response.Status = ServiceResponse.ServiceStatus.Created;
            response.Messages.Add("Comment added successfully.");
            return response;
        }

        public async Task<IEnumerable<CommentDto>> ListCommentsByTopic(int topicId)
        {
            return await _context.Comments
                .Where(c => c.TopicId == topicId)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new CommentDto
                {
                    CommentId = c.CommentId,
                    CommentText = c.CommentText,
                    CreatedAt = c.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                    UserName = "Anonymous"
                })
                .ToListAsync();
        }

        public async Task<ServiceResponse> DeleteComment(int commentId)
        {
            var response = new ServiceResponse();
            var comment = await _context.Comments.FindAsync(commentId);

            if (comment == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Comment not found.");
                return response;
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            response.Status = ServiceResponse.ServiceStatus.Deleted;
            response.Messages.Add("Comment deleted successfully.");
            return response;
        }

        public async Task<ServiceResponse> UpdateComment(int commentId, string newText)
        {
            var response = new ServiceResponse();

            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Comment not found.");
                return response;
            }

            comment.CommentText = newText;

            try
            {
                await _context.SaveChangesAsync();
                response.Status = ServiceResponse.ServiceStatus.Updated;
                response.Messages.Add("Comment updated successfully.");
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error updating comment.");
                response.Messages.Add(ex.Message);
            }

            return response;
        }
        public async Task<IEnumerable<CommentDto>> GetCommentsForTopic(int topicId)
        {
            var comments = await _context.Comments
                .Where(c => c.TopicId == topicId)
                .Select(c => new CommentDto
                {
                    CommentId = c.CommentId,
                    CommentText = c.CommentText,
                    CreatedAt = c.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                    UserName = "Anonymous"
                })
                .ToListAsync();

            return comments;
        }

    }
}

