using WebApplication1.Application.DTOs.GroupJoinRequest;
using WebApplication1.Shared.Results;

namespace WebApplication1.Application.Interfaces
{
    public interface IGroupJoinRequestService
    {
        Task<ApiResponse<GroupJoinRequestDto>> CreateGroupJoinRequestAsync(CreateGroupJoinRequestDto dto, string requestingUserId);
        Task<ApiResponse<string>> RespondToGroupJoinRequestAsync(Guid requestId, bool accept, string currentUserId);
        Task<PaginatedResult<GroupJoinRequestDto>> GetPendingJoinRequestsForGroupAsync(Guid groupId, FilterParams filterParams, string currentUserId);
        Task<int> GetPendingJoinRequestCountForOwnerAsync(string ownerId);
        Task<PaginatedResult<GroupJoinRequestDto>> GetAllPendingJoinRequestsForOwnerAsync(FilterParams filterParams, string ownerId);
    }
}
