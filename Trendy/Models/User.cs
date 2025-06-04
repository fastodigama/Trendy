using System.ComponentModel.DataAnnotations;

namespace Trendy.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public DateTime CreatedAt { get; set; }

        //A user can have many comments

        public ICollection<Comment>? Comments { get; set; }
    }
}
