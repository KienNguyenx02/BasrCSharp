using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Domain.Entities
{
    public class EventsEnitity : BaseEntity
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Location { get; set; }
        public string? Organizer { get; set; }
        public string? Priority { get; set; }
        public Guid GroupId { get; set; }
        public List<Guid> Attendees { get; set; } = new List<Guid>();
    }
}