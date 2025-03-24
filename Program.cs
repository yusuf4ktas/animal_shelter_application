using animal_shelter_app.Models;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// 1. Load .env file with DotNetEnv
Env.Load();

// 2. Also add environment variables to configuration
builder.Configuration.AddEnvironmentVariables();

// 3. Retrieve MySQL connection string from environment variables or appsettings
var myConnStr = builder.Configuration.GetConnectionString("MySqlConnection");
Console.WriteLine("Connection String from .env or config: " + myConnStr);

// 4. Configure DbContext for MySQL + EF Core
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(myConnStr, ServerVersion.AutoDetect(myConnStr))
);

// 5. Register HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// 6. Add MVC + Session
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Build the app
var app = builder.Build();

// Seed an admin user if none exists
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

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Enable session before authorization
app.UseSession();
app.UseAuthorization();

//Default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

//Run the application
app.Run();
