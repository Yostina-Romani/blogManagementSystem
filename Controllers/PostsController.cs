using BlogManagementSystem.Data;
using BlogManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlogManagementSystem.Controllers
{
    public class PostsController : Controller
    {
        private readonly BlogDbcontext _context;
        public PostsController(BlogDbcontext context)
        {
            _context=context;
        }
        [HttpGet]
        public IActionResult post()
        {
            return View();
        }
        [HttpPost]
       public IActionResult post(Posts post)
        {
            post.createAt = DateTime.Now;
            
            _context.posts.Add(post);
            _context.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
    }
}
