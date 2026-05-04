using Microsoft.AspNetCore.Mvc;
using BlogManagementSystem.Data;
using BlogManagementSystem.Models;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;

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
            string salt = GenerateSalt();
            user.salt = salt;
            user.registerTime= DateTime.Now;
            user.UserPassword = CreateHash(user.UserPassword, salt);
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
        public IActionResult Login(Users user)
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
            return RedirectToAction("Index", "Home");

        }
    }
}