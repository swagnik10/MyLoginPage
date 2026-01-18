using MyApp.Domain;

namespace MyApp.Repositories;

public interface IUserProfileRepository
{
    bool ProfileExists(int userId);
    UserProfile? GetByUserId(int userId);
    void CreateProfile(UserProfile profile);
    void UpdateProfile(UserProfile profile);
}
