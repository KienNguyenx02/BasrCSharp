using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Application.DTOs.Groups
{
    public class UpdateGroupDto
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}