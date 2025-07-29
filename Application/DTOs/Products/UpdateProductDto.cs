using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Application.DTOs.Products
{
    public class UpdateProductDto
    {
        [Required]
        public string? Name { get; set; }
        public string? Description { get; set; }
        [Required]
        public decimal Price { get; set; }
    }
}