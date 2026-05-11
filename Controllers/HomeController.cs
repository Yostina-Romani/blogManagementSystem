using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BlogManagementSystem.Models;
using System.Reflection.Metadata;
using BlogManagementSystem.Data;
using BlogManagementSystem.ViewModels;

using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BlogManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BlogDbcontext _context;
        
        public HomeController(ILogger<HomeController> logger ,BlogDbcontext context)
        {
           
            _logger = logger;
            _context = context;
        }
        [HttpGet]
        public IActionResult Index(string search)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
           
            var posts = _context.posts.AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
                {
                if (currentUserId == null)
                {
                    TempData["ErrorRegister"] = "must be register or login first";
                    return RedirectToAction("Register", "Account");
                }

                posts = posts.Where(p => p.Title.Contains(search) || p.postContent.Contains(search));
                
            }
            var model = new HomeVM
            {
                posts = posts.Include(p => p.comments).ThenInclude(p=>p.user).ToList()
            };
            
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
