using System.ComponentModel.DataAnnotations;
using Microsoft.Identity.Client;

namespace Trendy.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }
        public string? CommentText { get; set; }
        public DateTime CreatedAt { get; set; }

        //A comment belongs to one topic

        public required virtual Topic Topic { get; set; }
        public int TopicId { get; set; }

        //A comment can have one user
        public  User? User { get; set; }
        public int? UserId { get; set; }

    }


    public class CommentDto
    {
        public int CommentId { get; set; }
        public string CommentText { get; set; } = "No comment provided.";
        public string CreatedAt { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");


        // Temporarily hardcode
        public string UserName { get; set; } = "Anonymous";
    }

    public class CreateCommentDto
    {
        // Defaults if user doesn't provide comment
        public string CommentText { get; set; } =  "No comment provided.";


        public int TopicId { get; set; }

        public DateTime CreatedAt { get; set; }
        public int? UserId { get; set; }
    }
}
