namespace WebApplication1.Application.DTOs.UserEventStatus
{
    public class UserEventStatusDto
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public Guid UserId { get; set; }
        public string Status { get; set; } // e.g., "Attending", "Interested", "Declined"
    }
}