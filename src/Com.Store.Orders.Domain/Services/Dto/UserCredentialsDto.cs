using System.ComponentModel.DataAnnotations;

namespace Com.Store.Orders.Domain.Services.Dto
{
    public class UserCredentialsDto
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required]
        [StringLength(8, ErrorMessage = "Password is too short.")]
        public string Password { get; set; }
    }
}
