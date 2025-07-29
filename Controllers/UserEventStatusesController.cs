using Microsoft.AspNetCore.Mvc;
using WebApplication1.Application.DTOs.UserEventStatus;
using WebApplication1.Application.Interfaces;
using WebApplication1.Shared.ErrorCodes;
using WebApplication1.Shared.Results;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserEventStatusesController : ControllerBase
    {
        private readonly IUserEventStatusService _userEventStatusService;

        public UserEventStatusesController(IUserEventStatusService userEventStatusService)
        {
            _userEventStatusService = userEventStatusService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<object>>> Get([FromQuery] FilterParams filterParams)
        {
            var result = await _userEventStatusService.GetUserEventStatusesAsync(filterParams);
            return Ok(ApiResponse<object>.Ok(result));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> GetById(Guid id)
        {
            var userEventStatusDto = await _userEventStatusService.GetUserEventStatusByIdAsync(id);
            if (userEventStatusDto == null)
            {
                return NotFound(ApiResponse<object>.Fail(ErrorCode.NotFound("UserEventStatus").Message));
            }
            return Ok(ApiResponse<object>.Ok(userEventStatusDto));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<object>>> Create([FromBody] CreateUserEventStatusDto createUserEventStatusDto)
        {
            var userEventStatusDto = await _userEventStatusService.CreateUserEventStatusAsync(createUserEventStatusDto);
            return CreatedAtAction(nameof(GetById), new { id = userEventStatusDto.Id }, ApiResponse<object>.Ok(userEventStatusDto));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> Update(Guid id, [FromBody] UpdateUserEventStatusDto updateUserEventStatusDto)
        {
            var result = await _userEventStatusService.UpdateUserEventStatusAsync(id, updateUserEventStatusDto);
            if (!result)
            {
                return NotFound(ApiResponse<string>.Fail(ErrorCode.NotFound("UserEventStatus").Message));
            }
            return Ok(ApiResponse<string>.Ok("UserEventStatus updated successfully."));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> Delete(Guid id)
        {
            var result = await _userEventStatusService.DeleteUserEventStatusAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse<string>.Fail(ErrorCode.NotFound("UserEventStatus").Message));
            }
            return Ok(ApiResponse<string>.Ok("UserEventStatus deleted successfully."));
        }
    }
}