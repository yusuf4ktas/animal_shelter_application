using animal_shelter_app.Models;

namespace animal_shelter_app.Factories
{
    public class AdminAuthFactory : IUserAuthFactory
    {
        private readonly ApplicationDbContext _context;

        public AdminAuthFactory(ApplicationDbContext context)
        {
            _context = context;
        }

        public IUserAuthService CreateAuthService() => new AdminUserAuthService(_context);
    }
}
