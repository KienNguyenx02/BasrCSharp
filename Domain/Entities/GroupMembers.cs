using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Domain.Entities
{
    public class GroupMembers : BaseEntity
    {
        public Guid GroupId { get; set; }
        public Guid UserId { get; set; }
        public GroupRole GroupRole { get; set; } 
    }

    public enum GroupRole
    {
        Admin = 1,
        Member =2,
        Guest =3    
    }
}