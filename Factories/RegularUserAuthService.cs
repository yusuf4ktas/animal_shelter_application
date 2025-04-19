using animal_shelter_app.Models;

namespace animal_shelter_app.Factories
{
    public class RegularUserAuthService : IUserAuthService
    {
        private readonly ApplicationDbContext _context;

        public RegularUserAuthService(ApplicationDbContext context)
        {
            _context = context;
        }

        public User Authenticate(string email, string password)
        {
            var hashedPassword = User.HashPassword(password);
            return _context.Users.FirstOrDefault(u => u.UserEmail == email && u.UserPassword == hashedPassword && !u.UserRole);
        }
    }
}
