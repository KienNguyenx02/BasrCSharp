using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Application.DTOs.Users
{
    public class UpdateApplicationUserDto
    {
        [Required]
        public string Id { get; set; }
        public string? UserName { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        public string? NewPassword { get; set; }
    }
}