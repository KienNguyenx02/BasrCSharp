using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Application.DTOs.Orders
{
    public class CreateOrderDto
    {
        [Required]
        public List<CreateOrderItemDto> OrderItems { get; set; }
    }
}