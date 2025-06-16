using System.ComponentModel.DataAnnotations;

namespace Trendy.Models
{
    public class Category
    {
        [Key]

        public int CategoryId { get; set; }


        // The name of the category, initialized with a default value.
        // Ensures every category has a valid name if none is provided.

       
        public required string CategoryName { get; set; } = "General Category";

        //A Category can have multiple entries in CategoryTopic(1:M)
        // A category can be linked to multiple topics, preventing null references.

        public ICollection<CategoryTopic> CategoryTopics { get; set; } = new List<CategoryTopic>();

        


    }

    public class CategoryDto
    {
        public int categoryId { get; set; }

        public required string categoryName { get; set; }
    }

    public class CreateCategoryDto
    {
        public required string categoryName { get; set; }
    }
}
