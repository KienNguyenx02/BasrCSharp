using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Application.DTOs.Reminders
{
    public class CreateReminderDto
    {
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public string Message { get; set; }
        [Required]
        public DateTime ReminderDateTime { get; set; }
    }
}