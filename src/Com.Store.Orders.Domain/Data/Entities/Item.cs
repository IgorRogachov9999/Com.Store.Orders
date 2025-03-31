using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Com.Store.Orders.Domain.Data.Entities
{
    [Table("items", Schema = "orders")]
    public class Item
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public string Image { get; set; }

        [Required]
        public int AvailableCount { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        [DefaultValue(false)]
        public bool IsDeleted { get; set; }

        public List<OrderItem> Items { get; set; } = new();
    }
}
