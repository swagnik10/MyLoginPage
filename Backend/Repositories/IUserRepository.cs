using MyApp.Domain;

namespace MyApp.Repositories;

public interface IUserRepository
{
    bool UserNameExists(string userName);
    UsersCredentials? GetByUserName(string userName);
    UsersCredentials? GetByCredentials(string userName, string password);
    void CreateUser(UsersCredentials user);
    UsersCredentials? GetById(int userId);
    void UpdateCredentials(UsersCredentials user);
}

