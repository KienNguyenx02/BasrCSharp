using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Application.DTOs.Notifications
{
    public class CreateNotificationDto
    {
        [Required]
        public string? Message { get; set; }
        public string? UserId { get; set; } // Optional, if notification is for a specific user
    }
}