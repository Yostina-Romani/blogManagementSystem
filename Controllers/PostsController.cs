using BlogManagementSystem.Data;
using BlogManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

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
                TempData["ErrorRegister"] = "must be register first";
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


        //Delete Post
        [HttpGet]
        public IActionResult DeletePost(int postsId)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId == null)
            {
                TempData["ErrorRegister"] = "must be register first";
                return RedirectToAction("Register", "Account");
            }
            
            var post = _context.posts.Find(postsId);
            var ownerId = post.userId;
            if (ownerId.ToString() != currentUserId.ToString())
            {
                TempData["Error"] = "you cannot delete this post";
                return RedirectToAction("Index", "Home");

            }
            return View(post);
        }

        [HttpPost]
        public IActionResult DeletePost(Posts post)
        {
            var postDeleted = _context.posts.Find(post.postsId);
            _context.posts.Remove(postDeleted);
            _context.SaveChanges();
            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        public async Task< IActionResult> Details(int postiD)
        {
            var post =await _context.posts.Include(p=>p.User).FirstOrDefaultAsync(p => p.postsId == postiD);
            return View(post);
        }

        public IActionResult ExportAllToPdf()
        {
            var posts = _context.posts.Include(p=>p.User).ToList();
            MemoryStream memory = new MemoryStream();
            Document pdf = new Document(PageSize.A4);
            PdfWriter.GetInstance(pdf, memory);
            pdf.Open();
            pdf.Add(new Paragraph("posts list"));
            pdf.Add(new Paragraph(" "));
            foreach( var post in posts)
            {
                pdf.Add(new Paragraph("title:" + post.Title));
                pdf.Add(new Paragraph("content:" + post.postContent));
                pdf.Add(new Paragraph("time:" + post.createAt));
                pdf.Add(new Paragraph("image:" + post.imageUrl));
                pdf.Add(new Paragraph("owner name:" + post.User?.UserName));
                pdf.Add(new Paragraph("owner email:" + post.User?.UserEmail));

            }
            pdf.Close();

            return File(memory.ToArray(), "application/pdf", "posts.pdf");
        }

        public IActionResult ExportOnePost(int postId)
        {
            var post = _context.posts.Include(p => p.User).FirstOrDefault(p=>p.postsId==postId);

            MemoryStream memory = new MemoryStream();
            Document pdf = new Document(PageSize.A4);
            PdfWriter.GetInstance(pdf, memory);
            pdf.Open();

            pdf.Add(new Paragraph("Post Details"));
            pdf.Add(new Paragraph(" "));
            pdf.Add(new Paragraph("title:" + post.Title));
            pdf.Add(new Paragraph("content:" + post.postContent));
            pdf.Add(new Paragraph("time:" + post.createAt));
            pdf.Add(new Paragraph("image:" + post.imageUrl));
            pdf.Add(new Paragraph("owner name:" + post.User?.UserName));
            pdf.Add(new Paragraph("owner email:" + post.User?.UserEmail));
            pdf.Close();

            return File(memory.ToArray(), "application/pdf", "post.pdf");

        }
    }
}
