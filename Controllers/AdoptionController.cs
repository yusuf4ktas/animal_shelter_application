using animal_shelter_app.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;

namespace animal_shelter_app.Controllers
{
    public class AdoptionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdoptionController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpPost]
        public IActionResult Adopt(int animalId, string adoptionDate, string adoptionTime, string userAddress)
        {
            // Check if the user is logged in by checking session or any other authentication method
            var userId = HttpContext.Session.GetInt32("UserId");

            if (!userId.HasValue)
            {
                // If not logged in, redirect to login page immediately
                return RedirectToAction("UserLogin", "Account"); // Direct the user to the login page
            }

            // Fetch the user from the database using the userId stored in the session
            var user = _context.Users.Find(userId.Value);

            if (user == null)
            {
                // If the user is not found in the database, clear the session and redirect to the login page
                HttpContext.Session.Clear();
                return RedirectToAction("UserLogin", "Account");
            }

            // Find the animal by its ID
            var animal = _context.AnimalInformations.Find(animalId);

            if (animal == null || animal.IsAdopted)
            {
                // If the animal does not exist or is already adopted, return an error
                return NotFound(); // Or any custom error page or message
            }

            // Combine the adoption date and time into a single DateTime value
            string adoptionDateTimeString = $"{adoptionDate} {adoptionTime}";

            // Try to parse the combined date and time string into DateTime
            if (!DateTime.TryParse(adoptionDateTimeString, out DateTime parsedAdoptionDateTime))
            {
                // If the date and time are not valid, return an error
                ModelState.AddModelError(string.Empty, "Invalid adoption date and/or time.");
                return View("Adopt", new { animalId = animalId });
            }

            // Create a new adoption record
            var adoption = new Adoption
            {
                UserId = user.UserId,
                AnimalId = animalId,
                AdoptionDate = parsedAdoptionDateTime, // Store the combined DateTime
                UserAddress = userAddress,
                AdoptionStatus = "Pending"
            };

            // Add the adoption record to the database
            _context.Adoptions.Add(adoption);            
            _context.SaveChanges();
        

            TempData["Message"] = "Your request has been submitted for admin approval.";
            // TempData["UserAlreadyAdopted"] = true;
            return RedirectToAction("SpecificAnimal", "Animal", new { id = animalId });


        }




        [HttpGet]
        public IActionResult UserAdoptions()
        {
           
            var userId = HttpContext.Session.GetInt32("UserId");
        
            var userRole = HttpContext.Session.GetString("UserRole");


            if (!userId.HasValue || userRole != "User")
            {
                // Logged out users or non-users should not access this page
                TempData["ErrorMessage"] = "You do not have access to this page.";
                return RedirectToAction("UserLogin", "Account"); 
            }

            
            int currentUserId = userId.Value;

            // Pull user adoption applications
            var adoptions = _context.Adoptions
                .Include(a => a.AnimalInformation) 
                .Where(a => a.UserId == currentUserId) 
                .ToList(); 

            // Views/Adoption/UserAdoptions.cshtml 
            return View(adoptions);
        }


        // Admin panelinden tüm başvuruları görmek için
        public IActionResult AdminAdoptions()
        {
            var adoptions = _context.Adoptions
                .Include(a => a.AnimalInformation)
                .Include(a => a.User)
                .Where(a => a.AdoptionStatus == "Pending") // <-- FİLTRELEME BURADA YAPILIYOR
                .ToList();

            return View(adoptions);
        }


        [HttpPost]
        [ValidateAntiForgeryToken] 
        public IActionResult ApproveAdoption(int adoptionId, string? adminNote)
        {
            // Admin rol kontrolü
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return Forbid(); 
            }

            var adoption = _context.Adoptions
                .Include(a => a.AnimalInformation)
                .Include(a => a.User)
                .FirstOrDefault(a => a.AdoptionId == adoptionId);

            if (adoption == null) return NotFound();

            // Prevents reprocessing of a request that has already been processed
            if (adoption.AdoptionStatus != "Pending")
            {
                TempData["ErrorMessage"] = "That adoption has already been processed.";
                
                return RedirectToAction("AdminAdoptions"); 
            }


            adoption.AdoptionStatus = "Approved";
            adoption.AdminNote = adminNote; 

            if (adoption.AnimalInformation != null)
            {
                adoption.AnimalInformation.IsAdopted = true; 
            }

            var user = _context.Users.Find(adoption.UserId);
            if (user != null)
            {
                user.PresentAnimals++; 
            }

            _context.SaveChanges();

            TempData["SuccessMessage"] = "Adoption process has been approved.";
           
            return RedirectToAction("AdminAdoptions"); 
        }


        [HttpPost]
        [ValidateAntiForgeryToken] 
        public IActionResult RejectAdoption(int adoptionId, string? adminNote)
        {
            // Admin rol kontrolü
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return Forbid(); 
            }

            var adoption = _context.Adoptions
                .Include(a => a.AnimalInformation)
                .Include(a => a.User)
                .FirstOrDefault(a => a.AdoptionId == adoptionId);

            if (adoption == null)
            {
                return NotFound();
            }

          
            if (adoption.AdoptionStatus != "Pending")
            {
                TempData["ErrorMessage"] = "That adoption has already been processed";
              
                return RedirectToAction("AdminAdoptions"); 
            }


            adoption.AdoptionStatus = "Rejected";
            adoption.AdminNote = adminNote; 

            _context.SaveChanges();

            TempData["SuccessMessage"] = "Adoption process has been rejected.";
            
            return RedirectToAction("AdminAdoptions"); 
        }



        // Check login status 
        public IActionResult CheckLoginStatus()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            return Json(new { isLoggedIn = userId.HasValue });
        }
    }
}
