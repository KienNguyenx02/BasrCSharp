using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Application.DTOs.Users
{
    public class UpdateUserProfileDto
    {
        public string? UserName { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? CurrentPassword { get; set; }
        public string? NewPassword { get; set; }
        // Add other profile properties to be updated
    }
}