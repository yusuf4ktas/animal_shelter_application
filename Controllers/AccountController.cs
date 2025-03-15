using Microsoft.AspNetCore.Mvc;
using animal_shelter_app.Models;

namespace animal_shelter_app.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public AccountController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        //GET: Editing the profile including the password
        [HttpGet]
        public IActionResult EditProfile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login");
            }

            var user = _dbContext.Users.Find(userId.Value);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            // Populate the view model with current user data
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
        // POST: Posting the edited profile to the db
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProfile(EditProfileViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login");
            }

            var existingUser = _dbContext.Users.Find(userId.Value);
            if (existingUser == null)
            {
                return RedirectToAction("Login");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Update user fields
            existingUser.UserName = model.UserName;
            existingUser.UserSurname = model.UserSurname;
            existingUser.UserEmail = model.UserEmail;
            existingUser.PresentAnimals = model.PresentAnimals;

            // Update password only if a new one was provided
            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                existingUser.UserPassword = animal_shelter_app.Models.User.HashPassword(model.NewPassword);
            }


            _dbContext.SaveChanges();

            // Redirect user to their respective page
            if (existingUser.UserRole)
            {
                // Admin
                return RedirectToAction("AdminPage");
            }
            else
            {
                // Normal user
                return RedirectToAction("UserPage");
            }
        }

        // GET: /Account/Register path
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register path
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(User userModel)
        {
            if (!ModelState.IsValid)
            {
                return View(userModel);
            }

            // Check if email is already registered
            if (_dbContext.Users.Any(u => u.UserEmail == userModel.UserEmail))
            {
                ModelState.AddModelError("", "Email already in use.");
                return View(userModel);
            }

            // Hash the password before saving
            userModel.UserPassword = animal_shelter_app.Models.User.HashPassword(userModel.UserPassword);

            // Set default role to user (false) and initialize present_animals to 0
            userModel.UserRole = false;
            userModel.PresentAnimals = 0;

            _dbContext.Users.Add(userModel);
            _dbContext.SaveChanges();

            return RedirectToAction("Login", "Account");
        }

        // GET: /Account/Login path
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login path
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string userEmail, string userPassword, string loginType)
        {
            if (string.IsNullOrWhiteSpace(userEmail) || string.IsNullOrWhiteSpace(userPassword))
            {
                ModelState.AddModelError("", "Email and Password are required.");
                return View();
            }

            // Hash the password for comparison
            string hashedPassword = animal_shelter_app.Models.User.HashPassword(userPassword);
            var user = _dbContext.Users
                .FirstOrDefault(u => u.UserEmail == userEmail && u.UserPassword == hashedPassword);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View();
            }

            // Check if the loginType is "Admin" but the user is not an admin
            if (loginType == "Admin" && !user.UserRole)
            {
                ModelState.AddModelError("", "You are not authorized as an admin.");
                return View();
            }

            // Set session info
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("UserEmail", user.UserEmail);
            HttpContext.Session.SetString("UserRole", user.UserRole ? "Admin" : "User");

            // If user is admin => go to AdminPage
            if (user.UserRole)
            {
                return RedirectToAction("AdminPage", "Account");
            }
            else
            {
                // Normal user => go to UserPage
                return RedirectToAction("UserPage", "Account");
            }
        }

        // End the session and redirect to the home page
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        // Helper function to check if the current session user is an admin.
        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("UserRole") == "Admin";
        }

        // GET: /Account/UserPage
        public IActionResult UserPage()
        {
            // Check if the current session user is actually a normal user
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "User")
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        // GET: /Account/AdminPage
        public IActionResult AdminPage()
        {
            //Check if the current session user is actually an admin
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        // Example of an admin-only dashboard for privileged actions.
        public IActionResult AdminDashboard()
        {
            if (!IsAdmin())
            {
                // Redirect back to login if the session is not opened for admin.
                return RedirectToAction("Login", "Account");
            }

            // Full access to your DB features.
            // Listing all users, manage animal records, etc.
            // var users = _dbContext.Users.ToList();
            // var animals = _dbContext.AnimalInformations.ToList();
            // ... and so on
            return View();
        }
    }
}
