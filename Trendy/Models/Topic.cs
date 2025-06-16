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

        // A topic can be linked to multiple categories through the CategoryTopic join table.
        // This collection stores all links to categories for this topic.
        public ICollection<CategoryTopic> CategoryTopics { get; set; } = new List<CategoryTopic>();


    }
    public class TopicDto
    {
        public int TopicId { get; set; }
        public string? TopicTitle { get; set; }
        public string? TopicDescription { get; set; }


        // A list of category names associated with this topic.
        // Used for display purposes in APIs or views.
        public List<string> TopicCategory { get; set; } = new List<string>();
        public string? CreatedAt { get; set; }
    }

    public class CreateTopicDto
    {
       
        public string? TopicTitle { get; set; }
        public string? TopicDescription { get; set; }

        // A Topic can also have multiple entries in CategoryTopic(1:M)
        // Initialized as an empty list to remove nullable green lines.
        public List<int> CategoryIds { get; set; } = new List<int>();

        public DateTime CreatedAt { get; set; }
        
    }

    public class UpdateTopicDto
    {
        public int TopicId { get; set; }
        public string TopicTitle { get; set; } = "New Topic";

        public string TopicDescription { get; set; } = "Topic Description";

        public List<int> CategoryIds { get; set; } = new List<int>(); // New category links


    }
}
