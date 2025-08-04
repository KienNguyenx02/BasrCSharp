using Microsoft.AspNetCore.Mvc;
using WebApplication1.Application.DTOs.Events;
using WebApplication1.Application.Interfaces;
using WebApplication1.Shared.ErrorCodes;
using WebApplication1.Shared.Results;
using Microsoft.AspNetCore.SignalR;
using WebApplication1.Hubs;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly IHubContext<EventHub> _hubContext;

        public EventsController(IEventService eventService, IHubContext<EventHub> hubContext)
        {
            _eventService = eventService;
            _hubContext = hubContext;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<object>>> Get([FromQuery] FilterParams filterParams)
        {
            var result = await _eventService.GetEventsAsync(filterParams);
            return Ok(ApiResponse<object>.Ok(result));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> GetById(Guid id)
        {
            var eventDto = await _eventService.GetEventByIdAsync(id);
            if (eventDto == null)
            {
                return NotFound(ApiResponse<object>.Fail(ErrorCode.NotFound("Event").Message));
            }
            return Ok(ApiResponse<object>.Ok(eventDto));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<object>>> Create([FromBody] CreateEventDto createEventDto)
        {
            var eventDto = await _eventService.CreateEventAsync(createEventDto);
            await _hubContext.Clients.All.SendAsync("ReceiveEventUpdate", "EventCreated", eventDto);
            return CreatedAtAction(nameof(GetById), new { id = eventDto.Id }, ApiResponse<object>.Ok(eventDto));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> Update(Guid id, [FromBody] UpdateEventDto updateEventDto)
        {
            var result = await _eventService.UpdateEventAsync(id, updateEventDto);
            if (!result)
            {
                return NotFound(ApiResponse<string>.Fail(ErrorCode.NotFound("Event").Message));
            }
            var updatedEvent = await _eventService.GetEventByIdAsync(id);
            await _hubContext.Clients.All.SendAsync("ReceiveEventUpdate", "EventUpdated", updatedEvent);
            return Ok(ApiResponse<string>.Ok("Event updated successfully."));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> Delete(Guid id)
        {
            var result = await _eventService.DeleteEventAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse<string>.Fail(ErrorCode.NotFound("Event").Message));
            }
            await _hubContext.Clients.All.SendAsync("ReceiveEventUpdate", "EventDeleted", id);
            return Ok(ApiResponse<string>.Ok("Event deleted successfully."));
        }
    }
}