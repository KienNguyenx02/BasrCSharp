using WebApplication1.Application.DTOs.Reminders;
using WebApplication1.Shared.Results;

namespace WebApplication1.Application.Interfaces
{
    public interface IReminderService
    {
        Task<PaginatedResult<ReminderDto>> GetRemindersAsync(FilterParams filterParams);
        Task<ReminderDto> GetReminderByIdAsync(Guid id);
        Task<ReminderDto> CreateReminderAsync(CreateReminderDto createReminderDto);
        Task<bool> UpdateReminderAsync(Guid id, UpdateReminderDto updateReminderDto);
        Task<bool> DeleteReminderAsync(Guid id);
    }
}