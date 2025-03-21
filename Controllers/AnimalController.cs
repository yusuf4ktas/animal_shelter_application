using animal_shelter_app.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace animal_shelter_app.Controllers
{
    public class AnimalController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AnimalController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Seçilen türe göre hayvanları listeleyen fonksiyon
        public IActionResult ListAnimal(string species)
        {
            var animals = _context.AnimalInformations
                                  .Where(a => a.Species == species && !a.IsAdopted)
                                  .ToList();
            ViewBag.Species = char.ToUpper(species[0]) + species.Substring(1);
            return View(animals);
        }
        // Hayvanların fotoğraflarını yüklemek için yazdığım method
        /*Gerçek yönlendirmeyi yapmak için pathi düzgün vermemiz gerekiyor şu an o pathte bir hayvan bulamadığı
        için Hayvan bulunamadı çıkıyor ekrana */
        [HttpPost]
        public IActionResult UploadImage(int id, IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                // Veritabanında güncelle
                var animal = _context.AnimalInformations.Find(id);
                if (animal != null)
                {
                    animal.AnimalImage = "/uploads/" + fileName;
                    _context.SaveChanges();
                }

                return RedirectToAction("Index");
            }

            return View();
        }
        public async Task<IActionResult> Index()
        {
            // Veritabanından hayvanları çek
            var animals = await _context.AnimalInformations.ToListAsync();

            // View'a gönder
            return View(animals);
        }

        // Seçilen hayvanın detaylarını gösteren fonksiyon
        public IActionResult SpecificAnimal(int id)
        {
            var animal = _context.AnimalInformations.Find(id);
            if (animal == null) return NotFound();

            var healthDetails = _context.AnimalHealthConditions
                                        .FirstOrDefault(h => h.AnimalId == id);

            if (healthDetails != null)
            {
                // Include health information in the animal model (Optional: You can map this if needed)
                animal.HealthCondition = healthDetails; // Assuming AnimalInformation has a HealthCondition property to store the related health info
            }

            return View(animal);
        }
    }
}
