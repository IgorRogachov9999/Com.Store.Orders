using Com.Store.Orders.Domain.Data.Enums;

namespace Com.Store.Orders.Domain.Services.Dto
{
    public class UserDto
    {
        public Guid Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public UserRole[] Roles { get; set; } = Array.Empty<UserRole>();
    }
}
