using Com.Store.Orders.Domain.Data.Entities;
using Com.Store.Orders.Domain.Data.Models;
using Com.Store.Orders.Domain.Data.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Com.Store.Orders.Domain.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DbSet<User> _users;

        public UserRepository(OrdersDbContext context)
        {
            _users = context.Users;
        }

        public Task<UserModel?> GetByEmailAndPasswordAsync(string email, string passwordHash, CancellationToken ct)
        {
            return _users
                .Where(x => x.Email == email && x.PasswordHash == passwordHash && !x.IsDeleted)
                .Select(x => new UserModel()
                {
                    Id = x.Id,
                    Username = x.Username,
                    Email = x.Email,
                    Roles = x.Roles
                })
                .FirstOrDefaultAsync(ct);
        }
    }
}
