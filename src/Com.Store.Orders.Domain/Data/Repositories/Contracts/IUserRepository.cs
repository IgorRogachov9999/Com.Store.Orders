using Com.Store.Orders.Domain.Data.Models;

namespace Com.Store.Orders.Domain.Data.Repositories.Contracts
{
    public interface IUserRepository
    {
        Task<UserModel?> GetByEmailAndPasswordAsync(string email, string passwordHash, CancellationToken ct);
    }
}
