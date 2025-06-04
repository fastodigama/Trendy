using System.ComponentModel.DataAnnotations;

namespace Trendy.Models
{
    public class Topic
    {
        [Key]
        public int TopicId { get; set; }
        public string? TopicTitle { get; set; }
        public string? TopicDescription { get; set; }
        public DateTime CreatedAt { get; set; }

        //A topic can have many comments
        public ICollection<Comment>? Comments { get; set; }
    }
}
