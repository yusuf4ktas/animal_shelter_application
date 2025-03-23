using animal_shelter_app.Models;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// Add HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Load .env file
Env.Load();
builder.Configuration.AddEnvironmentVariables();

// Get MySQL connection string
var myConnStr = builder.Configuration.GetConnectionString("MySqlConnection");
Console.WriteLine("Connection String from .env: " + myConnStr);

// Configure DbContext for MySQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(myConnStr, ServerVersion.AutoDetect(myConnStr))
);

// Configure MVC and Session
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout duration
    options.Cookie.HttpOnly = true; // Security
    options.Cookie.IsEssential = true; // Essential cookie
});

var app = builder.Build();

// Add admin user to database if not exists
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    if (!dbContext.Users.Any(u => u.UserRole == true))
    {
        var adminUser = new User
        {
            UserName = "Admin",
            UserSurname = "User",
            UserEmail = "admin@gmail.com",
            UserPassword = User.HashPassword("Admin4565"),
            UserRole = true,
            PresentAnimals = 0
        };

        dbContext.Users.Add(adminUser);
        dbContext.SaveChanges();
    }
}

// Error handling
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Middleware configuration
app.UseRouting();
app.UseSession(); // Ensure session is enabled before Authorization
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
