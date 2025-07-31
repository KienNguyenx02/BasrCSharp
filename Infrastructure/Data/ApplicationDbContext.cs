using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Domain.Entities;

namespace WebApplication1.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Notifications> Notifications { get; set; }
        public DbSet<EventsEnitity> Events { get; set; }
        public DbSet<GroupsEntity> Groups { get; set; }
        public DbSet<GroupMembers> GroupMembers { get; set; }

        public DbSet<Reminders> Reminders { get; set; }
        public DbSet<UserEventStatus> UserEventStatus { get; set; }
    }
}
