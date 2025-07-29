using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Application.DTOs.Groups
{
    public class CreateGroupDto
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}