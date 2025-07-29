using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Application.DTOs.GroupMembers
{
    public class CreateGroupMemberDto
    {
        [Required]
        public Guid GroupId { get; set; }
        [Required]
        public Guid UserId { get; set; }
        public DateTime JoinedDate { get; set; }
    }
}