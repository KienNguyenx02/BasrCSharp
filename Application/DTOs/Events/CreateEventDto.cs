using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Application.DTOs.Events
{
    public class CreateEventDto
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        public string Location { get; set; }
    }
}