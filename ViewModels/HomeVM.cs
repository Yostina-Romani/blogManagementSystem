using BlogManagementSystem.Models;  
namespace BlogManagementSystem.ViewModels
{
    public class HomeVM
    {
        public List<Posts> ?posts { get; set; }
        public Comments ?commentS { get; set; }
    }
}
