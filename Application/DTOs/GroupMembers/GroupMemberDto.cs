namespace WebApplication1.Application.DTOs.GroupMembers
{
    public class GroupMemberDto
    {
        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        public Guid UserId { get; set; }
        public DateTime JoinedDate { get; set; }
    }
}