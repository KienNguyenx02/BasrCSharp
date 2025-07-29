using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Domain.Entities
{
    public class UserEventStatus : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid EventId { get; set; }
        public EventStatus Status { get; set; } = EventStatus.Interested;
    }
    
    public enum EventStatus
    {
        Attending = 1,
        Interested = 2,
        Declined = 3
    }
}