using Microsoft.AspNetCore.Mvc;
using WebApplication1.Application.DTOs.Groups;
using WebApplication1.Application.Interfaces;
using WebApplication1.Shared.ErrorCodes;
using WebApplication1.Shared.Results;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroupsController : ControllerBase
    {
        private readonly IGroupService _groupService;

        public GroupsController(IGroupService groupService)
        {
            _groupService = groupService;
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
        public async Task<ActionResult<ApiResponse<object>>> Create([FromBody] CreateGroupDto createGroupDto)
        {
            var groupDto = await _groupService.CreateGroupAsync(createGroupDto);
            return CreatedAtAction(nameof(GetById), new { id = groupDto.Id }, ApiResponse<object>.Ok(groupDto));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> Update(Guid id, [FromBody] UpdateGroupDto updateGroupDto)
        {
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
            var result = await _groupService.DeleteGroupAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse<string>.Fail(ErrorCode.NotFound("Group").Message));
            }
            return Ok(ApiResponse<string>.Ok("Group deleted successfully."));
        }
    }
}