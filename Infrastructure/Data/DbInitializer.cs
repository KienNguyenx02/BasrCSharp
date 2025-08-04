
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Domain.Entities;

namespace WebApplication1.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            await context.Database.MigrateAsync();

            string[] roleNames = { "Admin", "Staff", "User" };
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Optional: Create a default Admin user for testing
            var adminEmail = "admin@example.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var newAdminUser = new ApplicationUser { UserName = "admin", Email = adminEmail, EmailConfirmed = true };
                var result = await userManager.CreateAsync(newAdminUser, "Admin@123"); // Use a strong password
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdminUser, "Admin");
                }
            }

            // Create 10 additional users (user6 to user16)
            for (int i = 6; i <= 16; i++)
            {
                var userName = $"user{i}";
                var userEmail = $"user{i}@example.com";
                var existingUser = await userManager.FindByEmailAsync(userEmail);
                if (existingUser == null)
                {
                    var newUser = new ApplicationUser { UserName = userName, Email = userEmail, EmailConfirmed = true };
                    var result = await userManager.CreateAsync(newUser, "Admin@123");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(newUser, "User");
                    }
                }
            }
        }
    }
}
