using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Domain.Entities
{
    public class Group : BaseEntity
    {
        public string GroupName { get; set; }
        public string? Description { get; set; }
        public string? Avt { get; set; }
        public int? MaxMembers { get; set; }

        // Foreign key to ApplicationUser
        public string OwnerId { get; set; }
        [ForeignKey("OwnerId")]
        public virtual ApplicationUser Owner { get; set; }

        public virtual ICollection<GroupMembers> GroupMembers { get; set; }
        public virtual ICollection<GroupJoinRequest> GroupJoinRequests { get; set; }
    }
}