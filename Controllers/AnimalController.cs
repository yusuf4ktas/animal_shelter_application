using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using animal_shelter_app.Models;

namespace animal_shelter_app.Controllers
{
    public class AnimalController : Controller
    {
        private readonly ApplicationDbContext _db;
        public AnimalController(ApplicationDbContext db) => _db = db;

        // GET: /Animal/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var all = await _db.AnimalInformations
                               .Include(a => a.HealthCondition)
                               .ToListAsync();
            return View(all);
        }

        // GET: /Animal/List
        [HttpGet]
        public IActionResult List(string species)
        {
            var q = _db.AnimalInformations.AsQueryable();
            if (!string.IsNullOrEmpty(species))
                q = q.Where(a => a.Species.Equals(species, System.StringComparison.OrdinalIgnoreCase));

            ViewBag.Species = string.IsNullOrEmpty(species)
                ? "All Animals"
                : CultureInfo.CurrentCulture.TextInfo.ToTitleCase(species);

            var list = q.Include(a => a.HealthCondition).ToList();
            return View("ListAnimal", list);
        }

        // GET: /Animal/SpecificAnimal/
        [HttpGet]
        public async Task<IActionResult> SpecificAnimal(int id)
        {
            var animal = await _db.AnimalInformations
                                  .Include(a => a.HealthCondition)
                                  .Include(a => a.AnimalType)
                                  .FirstOrDefaultAsync(a => a.AnimalId == id);
            if (animal == null) return NotFound();
            return View(animal);
        }
    }
}
