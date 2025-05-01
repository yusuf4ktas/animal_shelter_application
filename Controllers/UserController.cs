using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using animal_shelter_app.Models;
using System.Linq;
using System.Security.Claims;

namespace animal_shelter_app.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Constructor
        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Sahiplenme başvurularını listeleyen action
        public async Task<IActionResult> UserAdoptionProcessList()
        {
            // Kullanıcıya ait sahiplenme başvurularını alıyoruz
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Console.WriteLine("Current User Id: " + userIdString);
            if (!int.TryParse(userIdString, out int userId))
            {
                return Unauthorized(); // Kimlik çözülemedi
            }
             


            // Kullanıcı ID'sinin doğru şekilde alınıp alınmadığını kontrol et
            Console.WriteLine($"Logged in UserId: {userId}");

            // Eğer UserId string (örneğin e-posta) ise ve veritabanında int olan bir UserId ile karşılaştırılacaksa:
            if (int.TryParse(userIdString, out int parsedUserId))
            {
                // Eğer dönüşüm başarılı olduysa, kullanıcının başvurularını sorguluyoruz
                var adoptionRequests = await _context.Adoptions
                    .Where(ar => ar.UserId == parsedUserId) // Kullanıcının başvurularını getiriyoruz
                    .Include(ar => ar.AnimalInformation) // Hayvan bilgilerini de dahil ediyoruz
                    .ToListAsync();

                // Eğer başvuru yoksa mesaj göster
                if (adoptionRequests.Count == 0)
                {
                    ViewData["Message"] = "You have no adoption records yet";
                }

                return View(adoptionRequests);
            }
            else
            {
                // Eğer kullanıcı ID'si geçerli bir integer değilse, hata mesajı göster
                ViewData["Message"] = "Invalid User ID";
                return View();
            }
        }


    }
}





/*
// GET: /User/UserPage
public IActionResult UserPage()
{
    // Kullanıcının giriş bilgilerini alıyoruz (User.Identity.Name)
    var email = User.Identity.Name;

    // Kullanıcıyı veritabanından buluyoruz
    var user = _context.Users.FirstOrDefault(u => u.UserEmail == email);

    if (user == null)
    {
        // Kullanıcı bulunamazsa login sayfasına yönlendiriyoruz
        return RedirectToAction("Login", "Account");
    }

    // Kullanıcının sahiplenme başvurularını alıyoruz
    var userAdoptions = _context.Adoptions
        .Include(a => a.AnimalInformation) // Hayvan bilgilerini de dahil ediyoruz
        .Where(a => a.UserId == user.UserId)   // Kullanıcının başvurularını filtreliyoruz
        .ToList();

    // Başvuruları view'a model olarak gönderiyoruz
    return View(userAdoptions);
}

// Diğer UserController aksiyonları burada olabilir
}
}
*/