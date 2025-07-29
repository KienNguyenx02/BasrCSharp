using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Application.DTOs.Orders
{
    public class UpdateOrderDto
    {
        [Required]
        public string? Status { get; set; }
    }
}