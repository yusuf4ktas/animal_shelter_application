using animal_shelter_app.Models;

namespace animal_shelter_app.Factories
{
    public interface IUserAuthService
    {
        User Authenticate(string email, string password);
    }
}
