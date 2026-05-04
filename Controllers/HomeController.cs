using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BlogManagementSystem.Models;
using System.Reflection.Metadata;
using BlogManagementSystem.Data;
using BlogManagementSystem.ViewModels;

using Microsoft.EntityFrameworkCore;

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

        public IActionResult Index()
        {
            var model = new HomeVM
            {
                posts = _context.posts.Include(p => p.comments).ToList()
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
