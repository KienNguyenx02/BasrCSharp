using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Application.DTOs.Users
{
    public class CreateApplicationUserDto
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}