using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectThread.Migrations;
using ProjectThread.Models;
using System.Linq;
using System.Security.Claims;

namespace ProjectThread.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginAsync(IFormCollection collection)
        {
            string email = collection["Email"];
            string password = collection["Password"];

            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.Password == password);

            if (user != null)
            {
                user.LastLogin = DateTime.Now;
                _context.Users.Update(user);

                var claims = new List<Claim>
                {
                    new Claim("UserName", user.Name),
                    new Claim("UserEmail", user.Email),
                    new Claim("UserGUID", user.UserGUID.ToString()),
                    new Claim("UserID", user.UserID.ToString()),
                    new Claim("FriendID", "")
                };

                var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("MyCookieAuth", principal);

                return RedirectToAction("Friends", "Users");
            }

            else
            {
                ViewBag.Error = "Invalid email or password.";
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MyCookieAuth");
            return RedirectToAction("Login", "Auth");
        }


        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignUp(User user)
        {
            if (user != null)
            {
                string checkEmail = _context.Users.Where(a => a.Email == user.Email).Select(b => b.Email).FirstOrDefault();
                if (string.IsNullOrEmpty(checkEmail))
                {
                    _context.Users.Add(user);
                    _context.SaveChanges();
                    return RedirectToAction("Login", "Auth");
                }
                else
                {
                    ViewBag.Error = "A user allready exists with same email.";
                    return View();
                }

            }
            return View();
        }
    }
}
