using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Domain.Entities
{
    public class GroupInvitation : BaseEntity
    {
        [ForeignKey("Group")]
        public Guid GroupId { get; set; }
        public virtual Group Group { get; set; }

        [ForeignKey("Inviter")]
        public string InviterId { get; set; }
        [DeleteBehavior(DeleteBehavior.NoAction)]
        public virtual ApplicationUser Inviter { get; set; }

        [ForeignKey("InvitedUser")]
        public string InvitedUserId { get; set; }
        [DeleteBehavior(DeleteBehavior.NoAction)]
        public virtual ApplicationUser InvitedUser { get; set; }

        public InvitationStatus Status { get; set; }

        public DateTime DateSent { get; set; }
    }
}
