using System.ComponentModel.DataAnnotations;

namespace BlogManagementSystem.Models
{
    public class Posts
    {
        [Key]
      public int postsId { get; set; }
        [Required(ErrorMessage ="title is required")]
      public string Title { get; set; }
        [Required(ErrorMessage ="enter your post first")]
      public  string postContent { get; set; }
      public string ?imageUrl { get; set; }
        public DateTime createAt { get; set; }
      public int userId { get; set; }
      public virtual Users? User { get; set; }
        public virtual List<Comments>? comments { get; set; } = new();
    }
}