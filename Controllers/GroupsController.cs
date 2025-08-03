using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication1.Application.DTOs.Groups;
using WebApplication1.Application.Interfaces;
using WebApplication1.Domain.Entities; // Required for Group entity
using WebApplication1.Infrastructure.Data; // Required for IBaseRepository
using WebApplication1.Shared.ErrorCodes;
using WebApplication1.Shared.Results;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GroupsController : ControllerBase
    {
        private readonly IGroupService _groupService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IBaseRepository<Group> _groupRepository; // To fetch the group for authorization

        public GroupsController(IGroupService groupService, IAuthorizationService authorizationService, IBaseRepository<Group> groupRepository)
        {
            _groupService = groupService;
            _authorizationService = authorizationService;
            _groupRepository = groupRepository;
        }

        [HttpGet]
        [Route("my-groups")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<ApiResponse<object>>> GetMyGroups([FromQuery] FilterParams filterParams)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            var result = await _groupService.GetGroupsByOwnerIdAsync(userId, filterParams);
            return Ok(ApiResponse<object>.Ok(result));
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<object>>> Get([FromQuery] FilterParams filterParams)
        {
            var result = await _groupService.GetGroupsAsync(filterParams);
            return Ok(ApiResponse<object>.Ok(result));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> GetById(Guid id)
        {
            var groupDto = await _groupService.GetGroupByIdAsync(id);
            if (groupDto == null)
            {
                return NotFound(ApiResponse<object>.Fail(ErrorCode.NotFound("Group").Message));
            }
            return Ok(ApiResponse<object>.Ok(groupDto));
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<ApiResponse<object>>> Create([FromBody] CreateGroupDto createGroupDto)
        {
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(ownerId))
            {
                return Unauthorized();
            }

            var groupDto = await _groupService.CreateGroupAsync(createGroupDto, ownerId);
            return CreatedAtAction(nameof(GetById), new { id = groupDto.Id }, ApiResponse<object>.Ok(groupDto));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> Update(Guid id, [FromBody] UpdateGroupDto updateGroupDto)
        {
            var group = await _groupRepository.GetByIdAsync(id);
            if (group == null)
            {
                return NotFound(ApiResponse<string>.Fail(ErrorCode.NotFound("Group").Message));
            }

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, group, "IsGroupOwner");
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            var result = await _groupService.UpdateGroupAsync(id, updateGroupDto);
            if (!result)
            {
                return NotFound(ApiResponse<string>.Fail(ErrorCode.NotFound("Group").Message));
            }
            return Ok(ApiResponse<string>.Ok("Group updated successfully."));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> Delete(Guid id)
        {
            var group = await _groupRepository.GetByIdAsync(id);
            if (group == null)
            {
                return NotFound(ApiResponse<string>.Fail(ErrorCode.NotFound("Group").Message));
            }

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, group, "IsGroupOwner");
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            var result = await _groupService.DeleteGroupAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse<string>.Fail(ErrorCode.NotFound("Group").Message));
            }
            return Ok(ApiResponse<string>.Ok("Group deleted successfully."));
        }
    }
}