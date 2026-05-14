using System.ComponentModel.DataAnnotations;

namespace BlogManagementSystem.Models
{
    public class Users
    {
        [Key]
        public  int userId { get; set; }
        [Required(ErrorMessage ="must enter your name")]
        [MinLength(3)]
        [DataType(DataType.Text)]
        public string UserName { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "invalid email format")]

        public string UserEmail { get; set; }
        [Required][RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{7,}$", ErrorMessage = "password must be at least 7 chracters and contain uppercas,lowercase,number and special character")]

        public string UserPassword { get; set; }
        [Required]
        public string UserPassword_confirm { get; set; }

        public DateTime registerTime { get; set; }

        public string Role { get; set; } = "User";
        public string ?salt { get; set; }
        public virtual List<Posts> ?posts { get; set; } = new();
        public virtual List<Comments>? comments { get; set; } = new();
        
    }
}
