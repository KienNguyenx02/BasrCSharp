namespace WebApplication1.Application.DTOs.Reminders
{
    public class ReminderDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Message { get; set; }
        public DateTime ReminderDateTime { get; set; }
        public bool IsDismissed { get; set; }
    }
}