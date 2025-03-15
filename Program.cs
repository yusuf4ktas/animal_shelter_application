using animal_shelter_app.Models;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);
//Comment these parts if you have not added the .env file along with DotEnv package

// 1. Load the .env file
Env.Load(); // defaults to .env in the current directory

// 2. Also load environment variables into the configuration
builder.Configuration.AddEnvironmentVariables();

// 3. Read the connection string
var myConnStr = builder.Configuration.GetConnectionString("MySqlConnection");
Console.WriteLine("Connection String from .env: " + myConnStr);

// 4. Register your DbContext with EF
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        myConnStr,
        ServerVersion.AutoDetect(myConnStr)
    )
);

// Add services to the container.
builder.Services.AddControllersWithViews();

// For EF Core + MySQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("MySqlConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MySqlConnection"))
    )
);

// For Session (if you plan to store user info in session)
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    // For example, 30 minutes idle timeout
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});


var app = builder.Build();

// Seed the admin if it does not exist.
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // Check if an admin (UserRole = true) exists
    if (!dbContext.Users.Any(u => u.UserRole == true))
    {
        var adminUser = new User
        {
            UserName = "Admin",
            UserSurname = "User",
            UserEmail = "admin@gmail.com",
            UserPassword = User.HashPassword("Admin4565"),
            UserRole = true, // True means admin
            PresentAnimals = 0
        };

        dbContext.Users.Add(adminUser);
        dbContext.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Enable session
app.UseSession();

app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
