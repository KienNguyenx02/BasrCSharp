using WebApplication1.Application.DTOs.Groups;
using WebApplication1.Shared.Results;

namespace WebApplication1.Application.Interfaces
{
    public interface IGroupService
    {
        Task<PaginatedResult<GroupDto>> GetGroupsAsync(FilterParams filterParams);
        Task<GroupDto> GetGroupByIdAsync(Guid id);
        Task<GroupDto> CreateGroupAsync(CreateGroupDto createGroupDto);
        Task<bool> UpdateGroupAsync(Guid id, UpdateGroupDto updateGroupDto);
        Task<bool> DeleteGroupAsync(Guid id);
    }
}