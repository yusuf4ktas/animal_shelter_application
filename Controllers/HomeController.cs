using animal_shelter_app.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public IActionResult Index(string species, string neutered, string adopted,
                                     int? animal_gender, bool? disability_status,
                                     int? chronic_disease_status, string animal_age_range)
        {
            var query = _dbContext.AnimalInformations
                .Include(a => a.HealthCondition) // Ensure AnimalHealthCondition is loaded
                .AsQueryable();

            // Filter by species
            if (!string.IsNullOrEmpty(species))
            {
                query = query.Where(a => a.Species.ToLower() == species.ToLower());
            }

            // Filter by neutering status
            if (!string.IsNullOrEmpty(neutered))
            {
                if (bool.TryParse(neutered, out bool isNeutered))
                {
                    query = query.Where(a => a.NeuteringStatus == isNeutered);
                }
            }

            // Filter by adoption status
            if (!string.IsNullOrEmpty(adopted))
            {
                if (bool.TryParse(adopted, out bool isAdopted))
                {
                    query = query.Where(a => a.IsAdopted == isAdopted);
                }
            }

            // Filter by animal gender
            if (animal_gender.HasValue)
            {
                query = query.Where(a => a.AnimalGender == (animal_gender == 1));
            }

            // Filter by disability status
            if (disability_status.HasValue)
            {
                query = query.Where(a => a.HealthCondition != null && a.HealthCondition.DisabilityStatus == disability_status.Value);
            }

            // Filter by chronic disease status
            if (chronic_disease_status.HasValue)
            {
                query = query.Where(a => a.HealthCondition != null && a.HealthCondition.ChronicDiseaseStatus == (chronic_disease_status == 1));
            }

            // Filter by animal age range
            if (!string.IsNullOrEmpty(animal_age_range))
            {
                switch (animal_age_range)
                {
                    case "0-1":
                        query = query.Where(a => a.AnimalAge >= 0 && a.AnimalAge <= 1);
                        break;
                    case "2-3":
                        query = query.Where(a => a.AnimalAge >= 2 && a.AnimalAge <= 3);
                        break;
                    case "4-5":
                        query = query.Where(a => a.AnimalAge >= 4 && a.AnimalAge <= 5);
                        break;
                    case "6-more":
                        query = query.Where(a => a.AnimalAge >= 6);
                        break;
                }
            }

            var animals = query.ToList();

            return View(animals);
        }
    }
}