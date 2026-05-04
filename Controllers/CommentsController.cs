using BlogManagementSystem.Data;
using BlogManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlogManagementSystem.Controllers
{
    public class CommentsController : Controller
    {
        private readonly BlogDbcontext _context;
        public CommentsController(BlogDbcontext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(Comments comment)
        {
            comment.commentTime = DateTime.Now;
           
            var currentComment = comment.commentContent;
            if (currentComment == null)
            {
                TempData["error"] = "comment cannot be empty";

                return RedirectToAction("Index", "Home");
            }
            _context.comments.Add(comment);
            _context.SaveChanges();
           return RedirectToAction("Index", "Home");
        }
    }
}
