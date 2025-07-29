using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Domain.Entities
{
    public class GroupsEntity : BaseEntity
    {
        public string? GroupName { get; set; }
        public string? Description { get; set; }
        public string? Avt { get; set; }
    }
}