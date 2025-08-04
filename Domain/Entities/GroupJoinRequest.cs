using WebApplication1.Domain.Enums;

namespace WebApplication1.Domain.Entities
{
    public class GroupJoinRequest : BaseEntity
    {
        public Guid GroupId { get; set; }
        public virtual Group Group { get; set; }

        public string RequestingUserId { get; set; }
        public virtual ApplicationUser RequestingUser { get; set; }

        public JoinRequestStatus Status { get; set; } = JoinRequestStatus.Pending;

        public DateTime DateRequested { get; set; } = DateTime.UtcNow;
    }
}
