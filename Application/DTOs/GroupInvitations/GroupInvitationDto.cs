using WebApplication1.Domain.Enums;

namespace WebApplication1.Application.DTOs.GroupInvitations
{
    public class GroupInvitationDto
    {
        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        public string GroupName { get; set; }
        public string InviterId { get; set; }
        public string InviterUsername { get; set; }
        public string InvitedUserId { get; set; }
        public string InvitedUserUsername { get; set; }
        public InvitationStatus Status { get; set; }
        public DateTime DateSent { get; set; }
    }
}
