using Microsoft.AspNetCore.Mvc;
using WebApplication1.Application.DTOs.GroupMembers;
using WebApplication1.Application.Interfaces;
using WebApplication1.Shared.ErrorCodes;
using WebApplication1.Shared.Results;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroupMembersController : ControllerBase
    {
        private readonly IGroupMemberService _groupMemberService;

        public GroupMembersController(IGroupMemberService groupMemberService)
        {
            _groupMemberService = groupMemberService;
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
        public async Task<ActionResult<ApiResponse<object>>> Create([FromBody] CreateGroupMemberDto createGroupMemberDto)
        {
            var result = await _groupMemberService.CreateGroupMemberAsync(createGroupMemberDto);
            if (!result.Success)
            {
                return BadRequest(ApiResponse<object>.Fail(result.Message));
            }
            return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, ApiResponse<object>.Ok(result.Data));
        }

        [HttpPut("{id}")]
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