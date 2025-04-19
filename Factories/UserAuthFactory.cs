using animal_shelter_app.Models;

namespace animal_shelter_app.Factories
{
    public class UserAuthFactory : IUserAuthFactory
    {
        private readonly ApplicationDbContext _context;

        public UserAuthFactory(ApplicationDbContext context)
        {
            _context = context;
        }

        public IUserAuthService CreateAuthService() => new RegularUserAuthService(_context);
    }
}
