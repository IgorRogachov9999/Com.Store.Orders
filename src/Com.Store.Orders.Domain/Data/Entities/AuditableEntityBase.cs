using System.ComponentModel.DataAnnotations;

namespace Com.Store.Orders.Domain.Data.Entities
{
    public abstract class AuditableEntityBase
    {
        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
