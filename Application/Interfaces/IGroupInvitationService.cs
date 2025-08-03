using WebApplication1.Application.DTOs.GroupInvitations;
using WebApplication1.Shared.Results;

namespace WebApplication1.Application.Interfaces
{
    public interface IGroupInvitationService
    {
        Task<ApiResponse<GroupInvitationDto>> CreateGroupInvitationAsync(CreateGroupInvitationDto dto, string inviterId);
        Task<ApiResponse<string>> RespondToGroupInvitationAsync(Guid invitationId, string invitedUserId, RespondGroupInvitationDto dto);
        Task<PaginatedResult<GroupInvitationDto>> GetPendingInvitationsForUserAsync(string userId, FilterParams filterParams);
        Task<PaginatedResult<GroupInvitationDto>> GetSentInvitationsByInviterAsync(string inviterId, FilterParams filterParams);
        Task<Domain.Entities.GroupInvitation> GetInvitationByIdAsync(Guid invitationId);
    }
}
