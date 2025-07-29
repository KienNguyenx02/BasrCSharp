using WebApplication1.Application.DTOs.Events;
using WebApplication1.Shared.Results;

namespace WebApplication1.Application.Interfaces
{
    public interface IEventService
    {
        Task<PaginatedResult<EventDto>> GetEventsAsync(FilterParams filterParams);
        Task<EventDto> GetEventByIdAsync(Guid id);
        Task<EventDto> CreateEventAsync(CreateEventDto createEventDto);
        Task<bool> UpdateEventAsync(Guid id, UpdateEventDto updateEventDto);
        Task<bool> DeleteEventAsync(Guid id);
    }
}