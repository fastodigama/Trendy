using System.ComponentModel.DataAnnotations;

namespace Trendy.Models
{
    public class Category
    {
        [Key]

        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }

        // A category can be applied to many topics

        public ICollection<Topic>? Topics { get; set; }



    }
}
