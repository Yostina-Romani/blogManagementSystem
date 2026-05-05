using BlogManagementSystem.Data;
using BlogManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
            // to create post
            post.createAt = DateTime.Now;
            var userIdCurrent = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdCurrent == null)
            {
                TempData["Error"] = "must be register first";
               return RedirectToAction("Register", "Account");
            }
            post.userId = int.Parse(userIdCurrent);
            _context.posts.Add(post);
            _context.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        //Edit post
        [HttpGet]
        public IActionResult Edit(int postsId)
        {
            // to edit post
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId == null) {
                TempData["Error"] = "must be register or login first";
                return RedirectToAction("Index", "Home");
            }
            
            var post = _context.posts.Find(postsId);
            var ownerId = post.userId;
            if (ownerId.ToString() != currentUserId.ToString())
            {
                TempData["Error"] = "you cannot edit this post";
                return RedirectToAction("Index", "Home");
            }
            return View(post);
        }
        [HttpPost]
        public IActionResult Edit(Posts updatePost)
        {
            var post = _context.posts.Find(updatePost.postsId);
            if (updatePost.Title == null)
            {
                TempData["Error"] = "title cannot be empty";
                return View(updatePost);
            }
            post.Title = updatePost.Title;
            if (updatePost.postContent == null)
            {
                TempData["Error"] = "content cannot be empty";
                return View(updatePost);
            }
            post.postContent = updatePost.postContent;
            _context.SaveChanges();
            return RedirectToAction("Index", "Home");

        }

    }
}
