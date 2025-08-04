namespace WebApplication1.Application.DTOs.GroupJoinRequest
{
    public class GroupJoinRequestDto
    {
        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        public string GroupName { get; set; }
        public string RequestingUserId { get; set; }
        public string RequestingUserName { get; set; }
        public string Status { get; set; }
        public DateTime DateRequested { get; set; }
    }
}
