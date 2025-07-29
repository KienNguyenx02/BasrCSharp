using Microsoft.AspNetCore.Mvc;
using WebApplication1.Application.DTOs.Reminders;
using WebApplication1.Application.Interfaces;
using WebApplication1.Shared.ErrorCodes;
using WebApplication1.Shared.Results;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RemindersController : ControllerBase
    {
        private readonly IReminderService _reminderService;

        public RemindersController(IReminderService reminderService)
        {
            _reminderService = reminderService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<object>>> Get([FromQuery] FilterParams filterParams)
        {
            var result = await _reminderService.GetRemindersAsync(filterParams);
            return Ok(ApiResponse<object>.Ok(result));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> GetById(Guid id)
        {
            var reminderDto = await _reminderService.GetReminderByIdAsync(id);
            if (reminderDto == null)
            {
                return NotFound(ApiResponse<object>.Fail(ErrorCode.NotFound("Reminder").Message));
            }
            return Ok(ApiResponse<object>.Ok(reminderDto));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<object>>> Create([FromBody] CreateReminderDto createReminderDto)
        {
            var reminderDto = await _reminderService.CreateReminderAsync(createReminderDto);
            return CreatedAtAction(nameof(GetById), new { id = reminderDto.Id }, ApiResponse<object>.Ok(reminderDto));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> Update(Guid id, [FromBody] UpdateReminderDto updateReminderDto)
        {
            var result = await _reminderService.UpdateReminderAsync(id, updateReminderDto);
            if (!result)
            {
                return NotFound(ApiResponse<string>.Fail(ErrorCode.NotFound("Reminder").Message));
            }
            return Ok(ApiResponse<string>.Ok("Reminder updated successfully."));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> Delete(Guid id)
        {
            var result = await _reminderService.DeleteReminderAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse<string>.Fail(ErrorCode.NotFound("Reminder").Message));
            }
            return Ok(ApiResponse<string>.Ok("Reminder deleted successfully."));
        }
    }
}