using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Linq;
using animal_shelter_app.Models;
using System.Security.Cryptography;
using System.Text;

namespace animal_shelter_app.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public AccountController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: User Login Page
        [HttpGet]
        public IActionResult UserLogin() => View();

        // POST: User Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UserLogin(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "Email and Password are required.");
                return View();
            }

            var hashedPassword = Models.User.HashPassword(password);
            var user = _dbContext.Users.FirstOrDefault(u => u.UserEmail == email && u.UserPassword == hashedPassword && !u.UserRole);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View();
            }

            SetSession(user, "User");
            return RedirectToAction("UserPage");
        }

        // GET: Admin Login Page
        [HttpGet]
        public IActionResult AdminLogin() => View();

        // POST: Admin Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AdminLogin(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "Email and Password are required.");
                return View();
            }

            var hashedPassword = Models.User.HashPassword(password);
            var admin = _dbContext.Users.FirstOrDefault(u => u.UserEmail == email && u.UserPassword == hashedPassword && u.UserRole);

            if (admin == null)
            {
                ModelState.AddModelError("", "Invalid admin login attempt.");
                return View();
            }

            SetSession(admin, "Admin");
            return RedirectToAction("Index", "Home");
        }

        // GET: Register Page
        [HttpGet]
        public IActionResult Register() => View();

        // POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(User userModel, string confirmPassword)
        {
            if (!ModelState.IsValid)
                return View(userModel);

            if (string.IsNullOrWhiteSpace(confirmPassword) || userModel.UserPassword != confirmPassword)
            {
                ModelState.AddModelError("", "Passwords do not match.");
                return View(userModel);
            }

            if (_dbContext.Users.Any(u => u.UserEmail == userModel.UserEmail))
            {
                ModelState.AddModelError("", "Email already in use.");
                return View(userModel);
            }

            userModel.UserPassword = Models.User.HashPassword(userModel.UserPassword);
            userModel.UserRole = false; // Default to regular user
            userModel.PresentAnimals = 0;

            _dbContext.Users.Add(userModel);
            _dbContext.SaveChanges();

            TempData["SuccessMessage"] = "Registration successful! You can now log in.";
            return RedirectToAction("UserLogin");
        }

        // Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("UserLogin");
        }

        // GET: Edit Profile
        [HttpGet]
        public IActionResult EditProfile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
                return RedirectToAction("UserLogin");

            var user = _dbContext.Users.Find(userId.Value);
            if (user == null)
                return RedirectToAction("UserLogin");

            var viewModel = new EditProfileViewModel
            {
                UserId = user.UserId,
                UserName = user.UserName,
                UserSurname = user.UserSurname,
                UserEmail = user.UserEmail,
                PresentAnimals = user.PresentAnimals
            };

            return View(viewModel);
        }

        // POST: Edit Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProfile(EditProfileViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
                return RedirectToAction("UserLogin");

            var existingUser = _dbContext.Users.Find(userId.Value);
            if (existingUser == null)
                return RedirectToAction("UserLogin");

            if (!ModelState.IsValid)
                return View(model);

            existingUser.UserName = model.UserName;
            existingUser.UserSurname = model.UserSurname;
            existingUser.UserEmail = model.UserEmail;
            existingUser.PresentAnimals = model.PresentAnimals;

            if (!string.IsNullOrEmpty(model.NewPassword))
                existingUser.UserPassword = Models.User.HashPassword(model.NewPassword);

            _dbContext.SaveChanges();
            return RedirectToAction("UserPage");
        }

        // Helper: Set Session Data
        private void SetSession(User user, string role)
        {
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("UserName", user.UserName);
            HttpContext.Session.SetString("UserRole", role);
        }

        // User Page
        public IActionResult UserPage()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "User")
                return RedirectToAction("UserLogin");

            return View();
        }
    }
}
