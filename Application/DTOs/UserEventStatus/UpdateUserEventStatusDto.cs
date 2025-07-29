using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Application.DTOs.UserEventStatus
{
    public class UpdateUserEventStatusDto
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public Guid EventId { get; set; }
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public string Status { get; set; }
    }
}