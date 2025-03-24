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
        public IActionResult Adopt(int animalId)
        {
            // Check if user is logged in 
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                // Not logged in => Redirect to login page
                return RedirectToAction("UserLogin", "Account");
            }

            // Find user from database
            var user = _context.Users.Find(userId.Value);
            if (user == null)
            {
                // Session exists but user not found => force logout & redirect
                HttpContext.Session.Clear();
                return RedirectToAction("UserLogin", "Account");
            }

            // Find animal
            var animal = _context.AnimalInformations.Find(animalId);
            if (animal == null || animal.IsAdopted)
            {
                return NotFound();
            }

            // Create adoption record
            var adoption = new Adoption
            {
                UserId = user.UserId,
                AnimalId = animalId,
                AdoptionDate = DateTime.Now
            };
            _context.Adoptions.Add(adoption);

            // Mark animal as adopted
            animal.IsAdopted = true;

        
            user.PresentAnimals++;

           
            _context.SaveChanges();


          
            return RedirectToAction("UserPage", "Account");
        }
    }
}
