using BlogManagementSystem.Data;
using BlogManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;

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
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(Comments comment)
        {
            comment.commentTime = DateTime.Now;

            var currentComment = comment.commentContent;
            
            if (string.IsNullOrWhiteSpace(currentComment))
            {
                TempData["Error"] = "comment cannot be empty";

                return RedirectToAction("Index", "Home");
            }
            var userIdCurrent = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdCurrent))
            {
                TempData["Error"] = "must be register first";
                return RedirectToAction("Index", "Home");
            }
            comment.usersId = int.Parse(userIdCurrent);
                    _context.comments.Add(comment);
                    _context.SaveChanges();
                    return RedirectToAction("Index", "Home");
                
            
        }
        [HttpGet]
        public IActionResult EditComment( int commentId)
        {
            var CurrentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(CurrentUserId))
            {
                TempData["Error"] = "must be login or register first";
                return RedirectToAction("Index", "Home");
            }
            var comment = _context.comments.Find(commentId);
            var ownerId = comment.usersId;
            if (ownerId.ToString() != CurrentUserId.ToString())
            {
                TempData["Error"] = "you cannot edit this comment";
                return RedirectToAction("Index", "Home");
            }
            return View(comment);
        }
        [HttpPost]
        public IActionResult EditComment(Comments commentUpdate)
        {
            var UpdateComment = _context.comments.Find(commentUpdate.commentId);
            if (commentUpdate.commentContent == null) {
                TempData["error"] = "Comment cannot be empty";
                return View(commentUpdate);

            }
        
            UpdateComment.commentContent = commentUpdate.commentContent;
            _context.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult Delete_comment(int commentId)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value.ToString();
            var comment = _context.comments.Find(commentId);


            if (string.IsNullOrEmpty(currentUserId))
            {
                TempData["Error"] = "you must login or register first";
                return RedirectToAction("Index", "Home");
            }

            var OwnerId = comment.usersId.ToString();

                if (OwnerId != currentUserId)
                {
                    TempData["Error"] = "you cannot delete this comment";
                    return RedirectToAction("Index", "Home");

                
            }

                return View(comment);
            

        }
        [HttpPost]
        public IActionResult Delete_comment(Comments comment)
        {
            var comment_delete = _context.comments.Find(comment.commentId);
            _context.comments.Remove(comment_delete);
            _context.SaveChanges();

            return RedirectToAction("Index", "Home");


        }
    }
}
