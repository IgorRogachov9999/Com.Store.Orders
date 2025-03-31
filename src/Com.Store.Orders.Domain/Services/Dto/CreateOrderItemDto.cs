using System.ComponentModel.DataAnnotations;

namespace Com.Store.Orders.Domain.Services.Dto
{
    public class CreateOrderItemDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity can not be less then 1.")]
        public int Quantity { get; set; }
    }
}
