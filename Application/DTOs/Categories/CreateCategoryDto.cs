using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Application.DTOs.Categories
{
    public class CreateCategoryDto
    {
        [Required(ErrorMessage = "Category name is required.")]
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}