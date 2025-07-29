namespace WebApplication1.Application.DTOs.Users
{
    public class UserProfileDto
    {
        public Guid? Id { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        // Add other profile properties as needed
    }
}