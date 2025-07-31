using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Application.DTOs.Notifications
{
    public class UpdateNotificationDto
    {
        [Required]
        public Guid Id { get; set; }
        public string? Message { get; set; }
        public bool IsRead { get; set; }
    }
}