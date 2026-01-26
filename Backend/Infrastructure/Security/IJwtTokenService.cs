namespace MyApp.Infrastructure.Security;

public interface IJwtTokenService
{
    string GenerateToken(int userId, string userName);
}
