using System.ComponentModel.DataAnnotations;

namespace BlogManagementSystem.Models
{
    public class Comments
    {
        [Key]
     public   int commentId { get; set; }
       // [Required(ErrorMessage = "must enter your comment first")]

        public string commentContent { get; set; }
     public DateTime commentTime { get; set; }
     public int usersId { get; set; }
     public virtual Users? user { get; set; }
     public int postsId { get; set; } 
     public  virtual Posts ?post { get; set; }
    }
}
