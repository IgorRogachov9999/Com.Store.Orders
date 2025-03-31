using Com.Store.Orders.Domain.Data.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Com.Store.Orders.Domain.Data.Entities
{
    [Table("users", Schema = "orders")]
    public class User : AuditableEntityBase
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        [DefaultValue(false)]
        public bool IsDeleted { get; set; }

        public UserRole[] Roles { get; set; } = Array.Empty<UserRole>();
    }
}
