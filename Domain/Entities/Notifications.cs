using System;
using WebApplication1.Domain.Entities;

namespace WebApplication1.Domain.Entities
{
    public class Notifications : BaseEntity
    {
        public string? Message { get; set; }
        public string? Title { get; set; }
        public string? Link { get; set; }
        public bool IsRead { get; set; }
        public string? UserId { get; set; } // To link notification to a specific user
    }
}