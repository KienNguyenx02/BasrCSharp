using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using WebApplication1.Domain.Entities;
using WebApplication1.Infrastructure.Data;

namespace WebApplication1.Application.Authorization
{
    public class IsGroupOwnerHandler : AuthorizationHandler<IsGroupOwnerRequirement, Group>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public IsGroupOwnerHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, IsGroupOwnerRequirement requirement, Group resource)
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                context.Fail();
                return;
            }

            // Admins can do anything
            if (context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
                return;
            }

            // Check if the user is the owner of the group
            if (resource.OwnerId == userId)
            {
                context.Succeed(requirement);
                return;
            }

            context.Fail();
        }
    }
}
