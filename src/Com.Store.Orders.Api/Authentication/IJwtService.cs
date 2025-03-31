namespace Com.Store.Orders.Api.Authentication;

public interface IJwtService
{
    string GenerateToken(string userId, string[] roles);
}