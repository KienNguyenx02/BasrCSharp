using WebApplication1.Application.DTOs.UserEventStatus;
using WebApplication1.Shared.Results;

namespace WebApplication1.Application.Interfaces
{
    public interface IUserEventStatusService
    {
        Task<PaginatedResult<UserEventStatusDto>> GetUserEventStatusesAsync(FilterParams filterParams);
        Task<UserEventStatusDto> GetUserEventStatusByIdAsync(Guid id);
        Task<UserEventStatusDto> CreateUserEventStatusAsync(CreateUserEventStatusDto createUserEventStatusDto);
        Task<bool> UpdateUserEventStatusAsync(Guid id, UpdateUserEventStatusDto updateUserEventStatusDto);
        Task<bool> DeleteUserEventStatusAsync(Guid id);
    }
}