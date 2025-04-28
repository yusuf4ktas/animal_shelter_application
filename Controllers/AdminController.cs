using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using animal_shelter_app.Models;
using animal_shelter_app.Models.ViewModels;
using animal_shelter_app.Factories;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace animal_shelter_app.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;

        public AdminController(ApplicationDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        // GET: /Admin/AdminPage
        [HttpGet]
        public IActionResult AdminPage()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction(nameof(AdminLogin));

            // prefilled dates and species' dropdown
            var vm = new AddAnimalViewModel
            {
                ArrivalDate = DateTime.Today.Date,
                HealthCheckUpDate = DateTime.Today.Date,
                SpeciesList = GetSpeciesList()
            };
            return View(vm);
        }

        // POST: /Admin/AddAnimal
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult AddAnimal(AddAnimalViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                // re-populate species dropdown on redisplay
                vm.SpeciesList = GetSpeciesList();
                return View("AdminPage", vm);
            }

            //Insert AnimalInformation
            var ai = new AnimalInformation
            {
                AnimalAge = vm.AnimalAge,
                AnimalGender = vm.AnimalGender,
                NeuteringStatus = vm.NeuteringStatus,
                CharacteristicFeatures = vm.CharacteristicFeatures,
                PastInformation = vm.PastInformation,
                ArrivalDate = vm.ArrivalDate,
                Species = vm.Species, //Dog,Cat etc.
                IsAdopted = vm.IsAdopted
            };
            _db.AnimalInformations.Add(ai);
            _db.SaveChanges(); 

            // Save the uploaded image with respect to the naming rule
            var id = ai.AnimalId;
            var cleanName = vm.AnimalSpecies.Replace(" ", "_"); // Strip spaces from name and replae with "_"
            var ext = Path.GetExtension(vm.ImageFile.FileName);
            var fileName = $"{id}_{cleanName}_{vm.Species}{ext}";
            var dir = Path.Combine(_env.WebRootPath, "animals"); // animals folder in wwwroot
            Directory.CreateDirectory(dir);
            var fullPath = Path.Combine(dir, fileName);
            using var fs = new FileStream(fullPath, FileMode.Create);
            vm.ImageFile.CopyTo(fs);

            // update record with image URL
            ai.AnimalImage = "/animals/" + fileName;
            _db.SaveChanges();

            //AnimalHealthCondition Entitty Informations
            var hc = new AnimalHealthCondition
            {
                AnimalId = id,
                HealthCheckUpDate = vm.HealthCheckUpDate,
                DisabilityStatus = vm.DisabilityStatus,
                DisabilityDetails = vm.DisabilityStatus ? vm.DisabilityDetails : null,
                ChronicDiseaseStatus = vm.ChronicDiseaseStatus,
                ChronicDiseaseDetails = vm.ChronicDiseaseStatus ? vm.ChronicDiseaseDetails : null
            };
            _db.AnimalHealthConditions.Add(hc);

            //AnimalType (breed) special id code creation
            int baseCode = vm.Species.ToLower() switch
            {
                "dog" => 1000,
                "cat" => 2000,
                "rabbit" => 3000,
                "bird" => 4000,
                "hamster" => 5000,
                _ => 9000            // “other” – puts them in the 9000-range
            };

            var at = new AnimalType
            {
                // e.g. dog with AnimalId 7  -> 1000 + 7  = 1007
                AnimalSpecialTypeId = baseCode + ai.AnimalId,
                AnimalId = ai.AnimalId,           // FK to animal_information
                AnimalSpecies = vm.AnimalSpecies,      // “Labrador Retriever” …
                LifeExpectancy = vm.LifeExpectancy
            };

            _db.AnimalTypes.Add(at);

            _db.SaveChanges();

            TempData["Success"] = "Animal added successfully!"; // Temp messaage for informing

            return RedirectToAction("AdminPage", "Account");
        }

        // GET: /Admin/AdminLogin
        [HttpGet]
        public IActionResult AdminLogin() => View();

        // POST: /Admin/AdminLogin
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult AdminLogin(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "Email and password required");
                return View();
            }

            //AdminAuthFactory logic
            var admin = new AdminAuthFactory(_db)
                            .CreateAuthService()
                            .Authenticate(email, password);

            if (admin == null)
            {
                ModelState.AddModelError("", "Invalid login");
                return View();
            }

            HttpContext.Session.SetInt32("UserId", admin.UserId);
            HttpContext.Session.SetString("UserRole", "Admin");
            HttpContext.Session.SetString("UserName", admin.UserName);

            return RedirectToAction(nameof(AdminPage));
        }

        // Getting the species list for the dropdown
        private IEnumerable<SelectListItem> GetSpeciesList()
        {
            var raw = _db.AnimalInformations      
                       .AsNoTracking()
                       .Select(a => a.Species)
                       .ToList();             

            var distinct = raw.Where(s => !string.IsNullOrWhiteSpace(s))
                              .Select(s => s.Trim())
                              .Distinct(StringComparer.OrdinalIgnoreCase)
                              .OrderBy(s => s);

            return distinct.Select(s => new SelectListItem(s, s));
        }
    }
}
