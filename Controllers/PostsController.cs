using BlogManagementSystem.Data;
using BlogManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Http;

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
       public IActionResult post(Posts post,IFormFile imageFile)
        {
            // to create post
            var userIdCurrent = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdCurrent == null)
            {
                TempData["ErrorRegister"] = "must be register first";
               return RedirectToAction("Register", "Account");
            }
            if (imageFile != null)
            {
                string filename = Guid.NewGuid().ToString()+Path.GetExtension(imageFile.FileName);
                string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images", filename);
                using (var stream = new FileStream(fullPath, FileMode.Create)) 
                {
                    imageFile.CopyTo(stream);
                }
                post.imageUrl = "/Images/" + filename;
            }
            post.createAt = DateTime.Now;

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
        public IActionResult Edit(Posts updatePost ,IFormFile imageFile ,string removeImage)
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
            if (imageFile != null)
            {

                string filename = Guid.NewGuid().ToString()
                                  + Path.GetExtension(imageFile.FileName);

                string fullPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot/Images",
            filename
        );
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    imageFile.CopyTo(stream);
                }

                post.imageUrl = $"/Images/{filename}";
            }
            if(!string.IsNullOrEmpty(removeImage)&& removeImage == "true")
            {
                if (!string.IsNullOrEmpty(post.imageUrl))
                {
                    string oldPath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        post.imageUrl.TrimStart('/')
                    );

                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }

                post.imageUrl = null;
            }
            
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
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId == null)
            {
                TempData["ErrorRegister"] = "must be have an account";
                return RedirectToAction("Register", "Account");
            }
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
                pdf.Add(new Paragraph("owner name:" + post.User?.UserName));
                pdf.Add(new Paragraph("owner email:" + post.User?.UserEmail));
                if (!string.IsNullOrEmpty(post.imageUrl))
                {
                    string imagePath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        post.imageUrl.TrimStart('/')
                    );

                    if (System.IO.File.Exists(imagePath))
                    {
                        iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(imagePath);

                        img.ScaleToFit(300f, 300f); 
                        img.Alignment = Element.ALIGN_CENTER;

                        pdf.Add(img);
                    }
                }

            }
            pdf.Close();

            return File(memory.ToArray(), "application/pdf", "posts.pdf");
        }

        public IActionResult ExportOnePost(int postId)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId == null)
            {
                TempData["ErrorRegister"] = "must be have an account";
                return RedirectToAction("Register", "Account");
            }
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
            pdf.Add(new Paragraph("owner name:" + post.User?.UserName));
            pdf.Add(new Paragraph("owner email:" + post.User?.UserEmail));
            if (!string.IsNullOrEmpty(post.imageUrl))
            {
                string imagePath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    post.imageUrl.TrimStart('/')
                );

                if (System.IO.File.Exists(imagePath))
                {
                    iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(imagePath);

                    img.ScaleToFit(300f, 300f); 
                    img.Alignment = Element.ALIGN_CENTER;

                    pdf.Add(img);
                }
            }
            pdf.Close();

            return File(memory.ToArray(), "application/pdf", "post.pdf");

        }
    }
}
