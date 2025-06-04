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
        public required virtual User User { get; set; }
        public int UserId { get; set; }

    }
}
