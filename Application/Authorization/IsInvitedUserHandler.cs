using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using WebApplication1.Domain.Entities;
using WebApplication1.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Application.Authorization
{
    public class IsInvitedUserHandler : AuthorizationHandler<IsInvitedUserRequirement, GroupInvitation>
    {
        private readonly IBaseRepository<GroupInvitation> _invitationRepository;

        public IsInvitedUserHandler(IBaseRepository<GroupInvitation> invitationRepository)
        {
            _invitationRepository = invitationRepository;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, IsInvitedUserRequirement requirement, GroupInvitation resource)
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                context.Fail();
                return;
            }

            // Check if the current user is the invited user for this invitation
            if (resource.InvitedUserId == userId)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
        }
    }
}
