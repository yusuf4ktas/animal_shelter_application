using animal_shelter_app.Models;
using Microsoft.AspNetCore.Mvc;
using System;

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
                UserAddress = userAddress
            };

            // Add the adoption record to the database
            _context.Adoptions.Add(adoption);

            // Mark the animal as adopted
            animal.IsAdopted = true;

            // Update the user's adopted animal count (or any other necessary details)
            user.PresentAnimals++;

            // Save changes to the database
            _context.SaveChanges();

            // Redirect to the user's page after the adoption process
            return RedirectToAction("UserPage", "Account"); // Or the relevant page you want to redirect to
        }

        // Check login status 
        public IActionResult CheckLoginStatus()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            return Json(new { isLoggedIn = userId.HasValue });
        }
    }
}
