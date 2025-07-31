using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WebApplication1.Application.DTOs.Notifications;
using WebApplication1.Application.Interfaces;
using WebApplication1.Shared.ErrorCodes;
using WebApplication1.Shared.Results;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<object>>> Get([FromQuery] FilterParams filterParams)
        {
            var result = await _notificationService.GetNotificationsAsync(filterParams);
            return Ok(ApiResponse<object>.Ok(result));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> GetById(Guid id)
        {
            var notificationDto = await _notificationService.GetNotificationByIdAsync(id);
            if (notificationDto == null)
            {
                return NotFound(ApiResponse<object>.Fail(ErrorCode.NotFound("Notification").Message));
            }
            return Ok(ApiResponse<object>.Ok(notificationDto));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<object>>> Create([FromBody] CreateNotificationDto createNotificationDto)
        {
            var notificationDto = await _notificationService.CreateNotificationAsync(createNotificationDto);
            return CreatedAtAction(nameof(GetById), new { id = notificationDto.Id }, ApiResponse<object>.Ok(notificationDto));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> Update(Guid id, [FromBody] UpdateNotificationDto updateNotificationDto)
        {
            var result = await _notificationService.UpdateNotificationAsync(id, updateNotificationDto);
            if (!result)
            {
                return NotFound(ApiResponse<string>.Fail(ErrorCode.NotFound("Notification").Message));
            }
            return Ok(ApiResponse<string>.Ok("Notification updated successfully."));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> Delete(Guid id)
        {
            var result = await _notificationService.DeleteNotificationAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse<string>.Fail(ErrorCode.NotFound("Notification").Message));
            }
            return Ok(ApiResponse<string>.Ok("Notification deleted successfully."));
        }

        [HttpPut("mark-as-read/{id}")]
        public async Task<ActionResult<ApiResponse<string>>> MarkAsRead(Guid id)
        {
            var result = await _notificationService.MarkNotificationAsReadAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse<string>.Fail(ErrorCode.NotFound("Notification").Message));
            }
            return Ok(ApiResponse<string>.Ok("Notification marked as read successfully."));
        }
    }
}