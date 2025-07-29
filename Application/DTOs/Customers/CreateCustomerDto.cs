using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Application.DTOs.Customers
{
    public class CreateCustomerDto
    {
        [Required(ErrorMessage = "First Name is required.")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        public string? LastName { get; set; }

        [EmailAddress]
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
    }
}