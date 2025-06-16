using System.ComponentModel.DataAnnotations;

namespace Trendy.Models
{
    public class CategoryTopic
    {
        /// <summary>
        /// Represents the many-to-many relationship between Category and Topic.
        /// </summary>
        /// <remarks>
        /// This class serves as the junction table for linking Topics to Categories.
        /// Instead of each Topic directly having a CategoryId, or vice versa,  
        /// this table establishes a proper many-to-many mapping.
        /// 
        /// Each entry in this table links:
        /// - A single Category to a single Topic (1:M from the Category side).
        /// - A single Topic to a single Category (1:M from the Topic side).
        /// 
        /// This ensures that multiple Topics can belong to multiple Categories.
        /// </remarks>



        [Key]
        public int Id { get; set; }


        //Foreign key linking to Category
        public required virtual Category Category { get; set; }
        public int CategoryId { get; set; }

        //Foreign key linking to Topic
        public required virtual Topic Topic { get; set; }
        public int TopicId { get; set; }


    }
}
