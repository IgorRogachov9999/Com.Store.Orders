using Com.Store.Orders.Domain.Data.Enums;

namespace Com.Store.Orders.Domain.Data.Models
{
    public class UserModel
    {
        public Guid Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public UserRole[] Roles { get; set; } = Array.Empty<UserRole>();
    }
}
