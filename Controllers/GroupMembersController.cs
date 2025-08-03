using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Application.DTOs.GroupMembers;
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
    public class GroupMembersController : ControllerBase
    {
        private readonly IGroupMemberService _groupMemberService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IBaseRepository<Group> _groupRepository;

        public GroupMembersController(IGroupMemberService groupMemberService, IAuthorizationService authorizationService, IBaseRepository<Group> groupRepository)
        {
            _groupMemberService = groupMemberService;
            _authorizationService = authorizationService;
            _groupRepository = groupRepository;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<object>>> Get([FromQuery] FilterParams filterParams)
        {
            var result = await _groupMemberService.GetGroupMembersAsync(filterParams);
            return Ok(ApiResponse<object>.Ok(result));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> GetById(Guid id)
        {
            var groupMemberDto = await _groupMemberService.GetGroupMemberByIdAsync(id);
            if (groupMemberDto == null)
            {
                return NotFound(ApiResponse<object>.Fail(ErrorCode.NotFound("GroupMember").Message));
            }
            return Ok(ApiResponse<object>.Ok(groupMemberDto));
        }

        [HttpGet("ByGroup/{groupId}")]
        public async Task<ActionResult<ApiResponse<object>>> GetMembersByGroupId(Guid groupId)
        {
            var result = await _groupMemberService.GetGroupMembersByGroupIdAsync(groupId);
            return Ok(ApiResponse<object>.Ok(result));
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<ApiResponse<object>>> Create([FromBody] CreateGroupMemberDto createGroupMemberDto)
        {
            var group = await _groupRepository.GetByIdAsync(createGroupMemberDto.GroupId);
            if (group == null)
            {
                return NotFound(ApiResponse<object>.Fail("Group not found."));
            }

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, group, "IsGroupOwner");
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            var result = await _groupMemberService.CreateGroupMemberAsync(createGroupMemberDto);
            if (!result.Success)
            {
                return BadRequest(ApiResponse<object>.Fail(result.Message));
            }
            return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, ApiResponse<object>.Ok(result.Data));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<ApiResponse<string>>> Update(Guid id, [FromBody] UpdateGroupMemberDto updateGroupMemberDto)
        {
            var result = await _groupMemberService.UpdateGroupMemberAsync(id, updateGroupMemberDto);
            if (!result)
            {
                return NotFound(ApiResponse<string>.Fail(ErrorCode.NotFound("GroupMember").Message));
            }
            return Ok(ApiResponse<string>.Ok("GroupMember updated successfully."));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<ApiResponse<string>>> Delete(Guid id)
        {
            var result = await _groupMemberService.DeleteGroupMemberAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse<string>.Fail(ErrorCode.NotFound("GroupMember").Message));
            }
            return Ok(ApiResponse<string>.Ok("GroupMember deleted successfully."));
        }
    }
}