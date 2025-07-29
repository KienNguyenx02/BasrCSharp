using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Application.DTOs.Users
{
    public class UpdateUserProfileDto
    {
        [EmailAddress]
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        // Add other profile properties to be updated
    }
}