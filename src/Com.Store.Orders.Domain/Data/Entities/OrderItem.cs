using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Com.Store.Orders.Domain.Data.Entities
{
    [Table("order_item", Schema = "orders")]
    public class OrderItem
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid OrderId { get; set; }

        public Order Order { get; set; }

        [Required]
        public Guid ItemId { get; set; }

        public Item Item { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [DefaultValue(false)]
        public bool IsDeleted { get; set; }
    }
}
