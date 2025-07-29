using WebApplication1.Application.DTOs.GroupMembers;
using WebApplication1.Shared.Results;

namespace WebApplication1.Application.Interfaces
{
    public interface IGroupMemberService
    {
        Task<PaginatedResult<GroupMemberDto>> GetGroupMembersAsync(FilterParams filterParams);
        Task<GroupMemberDto> GetGroupMemberByIdAsync(Guid id);
        Task<GroupMemberDto> CreateGroupMemberAsync(CreateGroupMemberDto createGroupMemberDto);
        Task<bool> UpdateGroupMemberAsync(Guid id, UpdateGroupMemberDto updateGroupMemberDto);
        Task<bool> DeleteGroupMemberAsync(Guid id);
    }
}