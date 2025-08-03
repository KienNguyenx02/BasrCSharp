using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication1.Application.DTOs.GroupInvitations;
using WebApplication1.Application.Interfaces;
using WebApplication1.Infrastructure.Data; // Added this line
using WebApplication1.Shared.Results;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GroupInvitationsController : ControllerBase
    {
        private readonly IGroupInvitationService _groupInvitationService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IBaseRepository<Domain.Entities.Group> _groupRepository;

        public GroupInvitationsController(
            IGroupInvitationService groupInvitationService,
            IAuthorizationService authorizationService,
            IBaseRepository<Domain.Entities.Group> groupRepository)
        {
            _groupInvitationService = groupInvitationService;
            _authorizationService = authorizationService;
            _groupRepository = groupRepository;
        }

        [HttpPost]
        [Route("send")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<ApiResponse<GroupInvitationDto>>> SendInvitation([FromBody] CreateGroupInvitationDto dto)
        {
            var inviterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(inviterId))
            {
                return Unauthorized();
            }

            // Authorize that the inviter is the owner of the group
            var group = await _groupRepository.GetByIdAsync(dto.GroupId);
            if (group == null)
            {
                return NotFound(ApiResponse<GroupInvitationDto>.Fail("Group not found."));
            }

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, group, "IsGroupOwner");
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            var result = await _groupInvitationService.CreateGroupInvitationAsync(dto, inviterId);
            if (!result.Success)
            {
                return BadRequest(ApiResponse<GroupInvitationDto>.Fail(result.Message));
            }
            return Ok(ApiResponse<GroupInvitationDto>.Ok(result.Data));
        }

        [HttpPost]
        [Route("{invitationId}/respond")]
        public async Task<ActionResult<ApiResponse<string>>> RespondToInvitation(Guid invitationId, [FromBody] RespondGroupInvitationDto dto)
        {
            var invitedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(invitedUserId))
            {
                return Unauthorized();
            }

            var invitation = await _groupInvitationService.GetInvitationByIdAsync(invitationId);
            if (invitation == null)
            {
                return NotFound(ApiResponse<string>.Fail("Invitation not found."));
            }

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, invitation, "IsInvitedUser");
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            var result = await _groupInvitationService.RespondToGroupInvitationAsync(invitationId, invitedUserId, dto);
            if (!result.Success)
            {
                return BadRequest(ApiResponse<string>.Fail(result.Message));
            }
            return Ok(ApiResponse<string>.Ok(result.Message));
        }

        [HttpGet]
        [Route("pending")]
        public async Task<ActionResult<ApiResponse<PaginatedResult<GroupInvitationDto>>>> GetPendingInvitations([FromQuery] FilterParams filterParams)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            var result = await _groupInvitationService.GetPendingInvitationsForUserAsync(userId, filterParams);
            return Ok(ApiResponse<PaginatedResult<GroupInvitationDto>>.Ok(result));
        }

        [HttpGet]
        [Route("sent")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<ApiResponse<PaginatedResult<GroupInvitationDto>>>> GetSentInvitations([FromQuery] FilterParams filterParams)
        {
            var inviterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(inviterId))
            {
                return Unauthorized();
            }
            var result = await _groupInvitationService.GetSentInvitationsByInviterAsync(inviterId, filterParams);
            return Ok(ApiResponse<PaginatedResult<GroupInvitationDto>>.Ok(result));
        }
    }
}
