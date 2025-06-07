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

        //A topic can be applied to many categories?
        public ICollection<Category>? Categories { get; set; }


    }
    public class TopicDto
    {
        public int TopicId { get; set; }
        public string? TopicTitle { get; set; }
        public string? TopicDescription { get; set; }
        //each topic can have many categories
        public List<string>? TopicCategory { get; set; }
        public string? CreatedAt { get; set; }
    }

    public class CreateTopicDto
    {
       
        public string? TopicTitle { get; set; }
        public string? TopicDescription { get; set; }
        public List<int>? TopicCategory { get; set; }
        
    }
}
