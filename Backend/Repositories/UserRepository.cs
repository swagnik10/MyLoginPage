using MyApp.Domain;

namespace MyApp.Repositories;

public class UserRepository : IUserRepository
{
    private readonly NHibernate.ISession _session;

    public UserRepository(NHibernate.ISession session)
    {
        _session = session;
    }

    public bool UserNameExists(string userName)
    {
        return _session
            .Query<UsersCredentials>()
            .Any(u => u.UserName == userName);

    }

    public UsersCredentials? GetByUserName(string userName)
    {
        return _session
            .Query<UsersCredentials>()
            .FirstOrDefault(u => u.UserName == userName);
    }

    public UsersCredentials? GetByCredentials(string userName, string password)
    {
        return _session
            .Query<UsersCredentials>()
            .FirstOrDefault(u =>
                u.UserName == userName &&
                u.Password == password);
    }

    public void CreateUser(UsersCredentials user)
    {
        _session.Save(user);
    }
}

