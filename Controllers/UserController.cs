using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using animal_shelter_app.Models;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks; 

namespace animal_shelter_app.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        public UserController(ApplicationDbContext context) 
        {
            _context = context;
           
        }

        // Action listing adoption applications
        public async Task<IActionResult> UserAdoptionProcessList()
        {

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Console.WriteLine("Current User Id: " + userIdString);

            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login", "Account"); // Example redirect to Login
            }

            if (!int.TryParse(userIdString, out int userId))
            {
                ViewData["Message"] = "Invalid User ID format.";
                return View(Enumerable.Empty<Adoption>()); // Return empty list or error view
            }


            // Controlling userID 
            Console.WriteLine($"Logged in UserId: {userId}");


            // Querying the user's requests
            var adoptionRequests = await _context.Adoptions
                .Where(ar => ar.UserId == userId) 
                .Include(ar => ar.AnimalInformation) 
                .ToListAsync();

            if (adoptionRequests.Count == 0)
            {
                ViewData["Message"] = "You have no adoption records yet";
            }

            return View(adoptionRequests);
        }

        // Action to get user details for the modal
        [HttpGet] 
        public async Task<IActionResult> GetUserDetail(int userId) 
        {
            // Validate if userId is provided (int will be default 0 if not provided, check if 0 is valid)
            if (userId <= 0) 
            {
                return BadRequest(new { message = "Valid User ID is required." });
            }

            // Find the user by ID using the provided User model structure
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId); 

            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            // Get the number of animals owned by this user 
            var numberOfAnimalsOwned = user.PresentAnimals; 

            // Prepare the data to return as JSON using the User model's property names
            var userDetails = new
            {
                userId = user.UserId, 
                userName = user.UserName,
                userSurname = user.UserSurname, 
                userEmail = user.UserEmail,     
                numberOfAnimalsOwned = numberOfAnimalsOwned
                
            };

            // Returns the data as a JSON result
            return Ok(userDetails);
        }
    }
}
