using animal_shelter_app.Models;
using animal_shelter_app.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

public class ArticleController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IWebHostEnvironment _env;

    public ArticleController(ApplicationDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }
    
    //  GET  /Admin/PublishArticle
    [HttpGet]
    public IActionResult PublishArticle()
    {
        // Only admins:
        if (HttpContext.Session.GetString("UserRole") != "Admin")
            return RedirectToAction("AdminLogin", "Account");

        return View("PublishArticle", new AddArticleViewModel());
    }
    // GET: /Article/Blog
    [HttpGet]
    public IActionResult Blog()
    {
        var articles = _db.ArticleInformations
            .OrderByDescending(a => a.BlogDate)
            .ToList();

        return View("Blog", articles);
    }

    //  POST /Admin/PublishArticle
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> PublishArticle(AddArticleViewModel vm)
    {
        if (!ModelState.IsValid)
            return View("PublishArticle", vm);

        // save the image into  /wwwroot/slider
        var sliderDir = Path.Combine(_env.WebRootPath, "slider");
        Directory.CreateDirectory(sliderDir);

        var uniqueName = Guid.NewGuid() + Path.GetExtension(vm.ImageFile.FileName);
        var fullPath = Path.Combine(sliderDir, uniqueName);

        await using (var fs = new FileStream(fullPath, FileMode.Create))
            await vm.ImageFile.CopyToAsync(fs);

        // persist to DB
        var art = new ArticleInformations
        {
            BlogTitle = vm.BlogTitle,
            BlogContent = vm.BlogContent,
            BlogImage = "/slider/" + uniqueName,
            BlogUrl = vm.BlogUrl,
            BlogDate = DateTime.UtcNow.ToString("yyyy-MM-dd")   // store as ISO date
        };

        _db.ArticleInformations.Add(art);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Article published!";
        return RedirectToAction(nameof(PublishArticle));
    }
}