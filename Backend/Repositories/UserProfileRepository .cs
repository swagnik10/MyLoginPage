using MyApp.Domain;


namespace MyApp.Repositories;

public class UserProfileRepository : IUserProfileRepository
{
    private readonly NHibernate.ISession _session;

    public UserProfileRepository(NHibernate.ISession session)
    {
        _session = session;
    }

    public bool ProfileExists(int userId)
    {
        return _session
            .Query<UserProfile>()
            .Any(p => p.UserId == userId);
    }

    public UserProfile? GetByUserId(int userId)
    {
        return _session
            .Query<UserProfile>()
            .FirstOrDefault(p => p.UserId == userId);
    }

    public void CreateProfile(UserProfile profile)
    {
        _session.Save(profile);
    }

    public void UpdateProfile(UserProfile profile)
    {
        _session.Update(profile);
    }
}

