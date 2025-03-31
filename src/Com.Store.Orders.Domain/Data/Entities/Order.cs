using Com.Store.Orders.Domain.Data.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Com.Store.Orders.Domain.Data.Entities
{
    [Table("orders", Schema = "orders")]
    public class Order : AuditableEntityBase
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string Address { get; set; }

        public string Notes { get; set; }

        [Required]
        public OrderStatus Status { get; set; }

        [Required]
        [DefaultValue(false)]
        public bool IsDeleted { get; set; }

        public List<OrderItem> Items { get; set; } = new();
    }
}
