using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Application.DTOs.Events
{
    public class UpdateEventDto
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string? Description { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        public string? Location { get; set; }
        public string? Priority { get; set; }
        [Required]
        public Guid GroupId { get; set; }
        public List<Guid> Attendees { get; set; } = new List<Guid>();
    }
}