using animal_shelter_app.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace animal_shelter_app.Controllers
{
    public class AnimalController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AnimalController(ApplicationDbContext context) => _context = context;

        // GET: /Animal/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var animals = await _context.AnimalInformations
                                        .Include(a => a.HealthCondition)
                                        .ToListAsync();
            return View(animals);
        }

        // GET: /Animal/List?species
        [HttpGet]
        public IActionResult List(string species)
        {
            var query = _context.AnimalInformations.AsQueryable();
            if (!string.IsNullOrEmpty(species))
                query = query.Where(a => a.Species.Equals(species, StringComparison.OrdinalIgnoreCase));

            ViewBag.Species = string.IsNullOrEmpty(species)
                ? "All Animals"
                : CultureInfo.CurrentCulture.TextInfo.ToTitleCase(species);

            return View("ListAnimal", query.ToList());
        }


        // GET: /Animal/SpecificAnimal
        [HttpGet]
        public async Task<IActionResult> SpecificAnimal(int id)
        {
            var animal = await _context.AnimalInformations
                                       .Include(a => a.HealthCondition)
                                       .Include(a => a.AnimalSpecies)
                                       .FirstOrDefaultAsync(a => a.AnimalId == id);

            if (animal == null) return NotFound();
            return View(animal);
        }

        // POST: /Animal/UploadImage
        [HttpPost]
        public async Task<IActionResult> UploadImage(int id, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file selected.");

            var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "animals");
            Directory.CreateDirectory(uploads);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploads, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
                await file.CopyToAsync(stream);

            var animal = await _context.AnimalInformations.FindAsync(id);
            if (animal != null)
            {
                animal.AnimalImage = $"/animals/{fileName}";
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}