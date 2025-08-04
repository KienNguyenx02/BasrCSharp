using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Application.DTOs.GroupJoinRequest;
using WebApplication1.Application.Interfaces;
using System.Security.Claims;
using WebApplication1.Shared.Results;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/group-join-requests")]
    [Authorize]
    public class GroupJoinRequestsController : ControllerBase
    {
        private readonly IGroupJoinRequestService _joinRequestService;

        public GroupJoinRequestsController(IGroupJoinRequestService joinRequestService)
        {
            _joinRequestService = joinRequestService;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<GroupJoinRequestDto>>> Create([FromBody] CreateGroupJoinRequestDto dto)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized(ApiResponse<GroupJoinRequestDto>.Fail("User not authenticated."));
            }

            var result = await _joinRequestService.CreateGroupJoinRequestAsync(dto, currentUserId);
            if (!result.success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("{requestId}/respond")]
        public async Task<ActionResult<ApiResponse<string>>> Respond(Guid requestId, [FromQuery] bool accept)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized(ApiResponse<string>.Fail("User not authenticated."));
            }

            var result = await _joinRequestService.RespondToGroupJoinRequestAsync(requestId, accept, currentUserId);
            if (!result.success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("pending/{groupId}")]
        public async Task<ActionResult<PaginatedResult<GroupJoinRequestDto>>> GetPendingRequests(Guid groupId, [FromQuery] FilterParams filterParams)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized();
            }

            var result = await _joinRequestService.GetPendingJoinRequestsForGroupAsync(groupId, filterParams, currentUserId);
            return Ok(result);
        }

        [HttpGet("count/pending")]
        public async Task<ActionResult<ApiResponse<int>>> GetPendingRequestsCount()
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized(ApiResponse<int>.Fail("User not authenticated."));
            }

            var count = await _joinRequestService.GetPendingJoinRequestCountForOwnerAsync(currentUserId);
            return Ok(ApiResponse<int>.Ok(count));
        }

        [HttpGet("all/pending")]
        public async Task<ActionResult<PaginatedResult<GroupJoinRequestDto>>> GetAllPendingRequests([FromQuery] FilterParams filterParams)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized();
            }

            var result = await _joinRequestService.GetAllPendingJoinRequestsForOwnerAsync(filterParams, currentUserId);
            return Ok(result);
        }
    }
}