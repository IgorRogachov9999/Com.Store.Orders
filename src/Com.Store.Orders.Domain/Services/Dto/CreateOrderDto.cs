using System.ComponentModel.DataAnnotations;

namespace Com.Store.Orders.Domain.Services.Dto
{
    public class CreateOrderDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string Phone { get; set; }

        [Required]
        public string Address { get; set; }

        public string Notes { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "Order must contain at least 1 item.")]
        public List<CreateOrderItemDto> Items { get; set; } = new();
    }
}
