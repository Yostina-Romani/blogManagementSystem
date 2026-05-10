using BlogManagementSystem.Data;
using BlogManagementSystem.Models;
using BlogManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BlogManagementSystem.Controllers
{
    public class AccountController : Controller
    {  
        private readonly BlogDbcontext _context;
        public AccountController(BlogDbcontext context)
        {
            _context = context;
        }
        

     



        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
       
        private string GenerateSalt()
        {
            byte[] saltByte = new byte[16];
            using (var ran = RandomNumberGenerator.Create())
            {
                ran.GetBytes(saltByte);
            }
            return Convert.ToBase64String(saltByte);
        }
        private string CreateHash(string password, String salt)
        {
            using (var hashing = SHA256.Create())
            {
                string combine = password + salt;
                byte[] bytes = Encoding.UTF8.GetBytes(combine);
                bytes = hashing.ComputeHash(bytes);
                return Convert.ToBase64String(bytes);

            }

        }
        [HttpPost]
        public IActionResult Register(Users user)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);

                return View(user);
            }
            var emaiExist = _context.users.FirstOrDefault(u => u.UserEmail == user.UserEmail);
            if (emaiExist != null)
            {
                ModelState.AddModelError("UserEmail", "email already exist");
                return View(user);
            }
            var nameExist = _context.users.FirstOrDefault(u => u.UserName == user.UserName);
            if (nameExist != null)
            {
                ModelState.AddModelError("UserName", "name used");
                return View(user);
            }
         var   confirm_pass = user.UserPassword_confirm;
            var pass = user.UserPassword;
            if (pass != confirm_pass)
            {
                TempData["ErrorRegister"] = "The password and confirmation must match.";
                return View(user);

            }
           // if()
            string salt = GenerateSalt();
            user.salt = salt;
            user.registerTime= DateTime.Now;
            
            user.UserPassword = CreateHash(pass, salt);
            _context.users.Add(user);
            _context.SaveChanges();
            return RedirectToAction("Login");

        }
         


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task< IActionResult> Login(Users user)
        { 

            var dbuser = _context.users.FirstOrDefault(u => u.UserEmail == user.UserEmail);
            if (dbuser == null)
            {
                 ModelState.AddModelError("UserEmail", "email not found");
                return View(user);
            }
            string hashpass = CreateHash(user.UserPassword, dbuser.salt);
            if (hashpass != dbuser.UserPassword)
            {
                ModelState.AddModelError("UserPassword", "wrong password");
                return View(user);
            }
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,dbuser.userId.ToString()),
                new Claim(ClaimTypes.Name,dbuser.UserName)


            };
            var claimIdentifier = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authuProperties = new AuthenticationProperties();
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimIdentifier), authuProperties);
            return RedirectToAction("Index", "Home");

        }

        //lougout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
           return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult MyPosts()
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId == null)
            {
                TempData["ErrorRegister"] = "You donot have account";
                return RedirectToAction("Register", "Account");
            }
                var posts = _context.posts.Include(p=>p.comments).ThenInclude(p=>p.user).Where(p => p.userId == int.Parse(currentUserId)).ToList();

            HomeVM vm = new HomeVM()
            {
                posts = posts
            };
            return View(vm);
            
        }

    }
}