namespace WebApplication1.Application.DTOs.GroupInvitations
{
    public class CreateGroupInvitationDto
    {
        public Guid GroupId { get; set; }
        public string InvitedUserEmail { get; set; } // Or Username
    }
}
