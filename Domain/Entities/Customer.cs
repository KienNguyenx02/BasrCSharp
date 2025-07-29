using WebApplication1.Domain.Entities;

namespace WebApplication1.Domain.Entities
{
    public class Customer : BaseEntity
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }

        public Role UserRole { get; set; } = Role.User;
        public string? ProfilePicture { get; set; }
    }
    public enum Role
    {
        Admin,
        User,
        Staff   
    }
}