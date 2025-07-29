using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Domain.Entities
{
    public class Reminders : BaseEntity
    {
        public string? Title { get; set; } = null!;
        public Guid EnventId { get; set; }
        public Guid UserId { get; set; }
        public string? Description { get; set; }
        public DateTime ReminderDate { get; set; }
        public TimeOnly ReminderTime { get; set; }
        public bool IsCompleted { get; set; } = false;
       
    }
}