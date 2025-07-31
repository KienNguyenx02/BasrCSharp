namespace WebApplication1.Application.DTOs.Groups
{
    public class GroupDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Permissions { get; set; }
        public int? MaxMembers { get; set; } // New: Maximum number of members
    }
}