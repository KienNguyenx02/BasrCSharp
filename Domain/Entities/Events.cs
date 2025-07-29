using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Domain.Entities
{
    public class EventsEnitity : BaseEntity
    {
        public string? EventName { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime EventDate { get; set; }

        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string? Location { get; set; } = null!;
        public string? Organizer { get; set; }
        
        public Guid GroupId { get; set; }
        
    }
}