namespace WebApplication1.Application.DTOs.Events
{
    public class EventDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Location { get; set; }
        public string? Priority { get; set; }
        public Guid GroupId { get; set; }
        public List<Guid> Attendees { get; set; } = new List<Guid>();
    }
}