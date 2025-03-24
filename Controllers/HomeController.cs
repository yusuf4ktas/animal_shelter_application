using animal_shelter_app.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq;

namespace animal_shelter_app.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public HomeController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: /Home/Index
        [HttpGet]
        public IActionResult Index(string species)
        {
            // Pull from DB 
            var animals = _dbContext.AnimalInformations
                .Where(a => !a.IsAdopted)
                .ToList();

            // Then filter in memory, ignore case
            if (!string.IsNullOrEmpty(species))
            {
                animals = animals
                    .Where(a => a.Species.Equals(species, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            return View(animals);
        }
    }
}
