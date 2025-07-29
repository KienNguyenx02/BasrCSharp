using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Application.DTOs.UserEventStatus
{
    public class CreateUserEventStatusDto
    {
        [Required]
        public Guid EventId { get; set; }
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public string Status { get; set; }
    }
}