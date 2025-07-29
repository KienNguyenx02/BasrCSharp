using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Application.DTOs.GroupMembers
{
    public class UpdateGroupMemberDto
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public Guid GroupId { get; set; }
        [Required]
        public Guid UserId { get; set; }
        public DateTime JoinedDate { get; set; }
    }
}