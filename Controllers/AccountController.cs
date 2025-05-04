using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Linq;
using animal_shelter_app.Models;
using animal_shelter_app.Factories;
using animal_shelter_app.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;

namespace animal_shelter_app.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IWebHostEnvironment _env;     // ← NEW

        public AccountController(ApplicationDbContext dbContext, IWebHostEnvironment env)
        {
            _dbContext = dbContext;
            _env = env;
        }
        //  GET  /Account/UserPage
        [HttpGet]
        public IActionResult UserPage()
        {
            if (HttpContext.Session.GetString("UserRole") != "User")
                return RedirectToAction(nameof(UserLogin));

            var sliderDir = Path.Combine(_env.WebRootPath, "slider");
            var sliderPaths = Directory.Exists(sliderDir)
                ? Directory.GetFiles(sliderDir)
                           .OrderByDescending(System.IO.File.GetCreationTime)
                           .Select(p => "/slider/" + Path.GetFileName(p))
                           .Take(5)
                           .ToList()
                : new List<string>();


            var articles = _dbContext.ArticleInformations
                              .OrderByDescending(a => a.ArticleId)  // newest first
                              .Take(6)
                              .ToList();


            var vm = new UserDashboardViewModel(sliderPaths, articles);
            return View("UserPage", vm);          // ↳  Views/Account/UserPage.cshtml
        }

        // GET: /Account/UserLogin
        [HttpGet]
        public IActionResult UserLogin()
        {
            // show "Registration successful" if we just came from Register
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"];
            }
            return View();
        }


        // POST: /Account/UserLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UserLogin(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.ErrorMessage = "Email and Password are required.";
                return View();
            }

            var factory = new UserAuthFactory(_dbContext);
            var authService = factory.CreateAuthService();
            var user = authService.Authenticate(email, password);

            if (user == null)
            {
                ViewBag.ErrorMessage = "Invalid email or password.";
                return View();
            }

            HttpContext.Session.SetString("UserName", user.UserName);
            HttpContext.Session.SetString("UserRole", user.UserRole.ToString().ToLower()); // "true" veya "false"

            SetSession(user, "User");

            return RedirectToAction(nameof(UserPage));
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return RedirectToAction(nameof(UserLogin));
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(User userModel, string confirmPassword)
        {
            // basic server‐side validation
            if (string.IsNullOrWhiteSpace(userModel.UserName) ||
                string.IsNullOrWhiteSpace(userModel.UserSurname) ||
                string.IsNullOrWhiteSpace(userModel.UserEmail) ||
                string.IsNullOrWhiteSpace(userModel.UserPassword))
            {
                ViewBag.RegistrationError = "All fields are required.";
                return View("UserLogin", userModel);
            }

            if (userModel.UserPassword != confirmPassword)
            {
                ViewBag.RegistrationError = "Passwords do not match.";
                return View("UserLogin", userModel);
            }

            if (_dbContext.Users.Any(u => u.UserEmail == userModel.UserEmail))
            {
                ViewBag.RegistrationError = "Email already in use.";
                return View("UserLogin", userModel);
            }

            userModel.UserPassword = Models.User.HashPassword(userModel.UserPassword);
            userModel.UserRole = false; // regular user
            userModel.PresentAnimals = 0;

            _dbContext.Users.Add(userModel);
            _dbContext.SaveChanges();

            TempData["RegistrationSuccess"] = "Registration successful! Please log in.";
            return RedirectToAction(nameof(UserLogin));
        }

        // GET: /Account/AdminLogin
        [HttpGet]
        public IActionResult AdminLogin()
        {
            return View();
        }

        // POST: /Account/AdminLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AdminLogin(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.ErrorMessage = "Email and Password are required.";
                return View();
            }

            var factory = new AdminAuthFactory(_dbContext);
            var authService = factory.CreateAuthService();
            var admin = authService.Authenticate(email, password);

            if (admin == null)
            {
                ViewBag.ErrorMessage = "Invalid email or password.";
                return View();
            }

            SetSession(admin, "Admin");
            return RedirectToAction(nameof(AdminPage));
        }

        // GET: /Account/AdminPage
        [HttpGet]
        public IActionResult AdminPage()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("AdminLogin");


            // adoptions panel for admin
            var pendingAdoptions = _dbContext.Adoptions
       .Where(a => a.AdoptionStatus == "Pending")
       .Select(a => new
       {
           AdoptionId = a.AdoptionId,
           UserId = a.UserId,
           AnimalId = a.AnimalId,
           Date = a.AdoptionDate,
           Address = a.UserAddress,
           UserName = a.User.UserName,
           PresentAnimals = a.User.PresentAnimals
       }).ToList();

            ViewBag.PendingAdoptions = pendingAdoptions;


            // Create and initialize the view model
            var viewModel = new AddAnimalViewModel
            {
                ArrivalDate = DateTime.Now,
                HealthCheckUpDate = DateTime.Now,
                // Initialize the species dropdown
                SpeciesList = _dbContext.AnimalInformations
                    .Select(t => new SelectListItem
                    {
                        Value = t.AnimalId.ToString(),
                        Text = t.Species
                    })
                    .ToList()
            };

            return View(viewModel);
        }



        // GET: /Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(UserLogin));
        }

        // GET: /Account/EditProfile
        [HttpGet]
        public IActionResult EditProfile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue) return RedirectToAction(nameof(UserLogin));

            var user = _dbContext.Users.Find(userId.Value);
            if (user == null) return RedirectToAction(nameof(UserLogin));

            var vm = new EditProfileViewModel
            {
                UserId = user.UserId,
                UserName = user.UserName,
                UserSurname = user.UserSurname,
                UserEmail = user.UserEmail,
                PresentAnimals = user.PresentAnimals
            };

            // will look for Views/Account/User/EditProfile.cshtml
            return View(vm);
        }

        // POST: /Account/EditProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProfile(EditProfileViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue) return RedirectToAction(nameof(UserLogin));

            var existingUser = _dbContext.Users.Find(userId.Value);
            if (existingUser == null) return RedirectToAction(nameof(UserLogin));

            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Please check the information and try again.";
                return View(model);
            }

            existingUser.UserName = model.UserName;
            existingUser.UserSurname = model.UserSurname;
            existingUser.UserEmail = model.UserEmail;
            existingUser.PresentAnimals = model.PresentAnimals;

            if (!string.IsNullOrEmpty(model.NewPassword))
                existingUser.UserPassword = Models.User.HashPassword(model.NewPassword);

            _dbContext.SaveChanges();
            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction(nameof(UserPage));
        }

        // ---  Session helpers ---
        private void SetSession(User u, string role)
        {
            HttpContext.Session.SetInt32("UserId", u.UserId);
            HttpContext.Session.SetString("UserName", u.UserName);
            HttpContext.Session.SetString("UserRole", role);
        }
    }
}