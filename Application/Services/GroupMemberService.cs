using AutoMapper;
using WebApplication1.Application.DTOs.GroupMembers;
using WebApplication1.Application.Interfaces;
using WebApplication1.Domain.Entities;
using Microsoft.EntityFrameworkCore; 
using WebApplication1.Shared.Results;
using WebApplication1.Shared.Extensions;
using WebApplication1.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Application.Services
{
    public class GroupMemberService : IGroupMemberService
    {
        private readonly IBaseRepository<GroupMembers> _groupMemberRepository;
        private readonly IBaseRepository<Group> _groupRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;

        public GroupMemberService(
            IBaseRepository<GroupMembers> groupMemberRepository, 
            IBaseRepository<Group> groupRepository, 
            UserManager<ApplicationUser> userManager,
            IMapper mapper, 
            INotificationService notificationService)
        {
            _groupMemberRepository = groupMemberRepository;
            _groupRepository = groupRepository;
            _userManager = userManager;
            _mapper = mapper;
            _notificationService = notificationService;
        }

        public async Task<PaginatedResult<GroupMemberDto>> GetGroupMembersAsync(FilterParams filterParams)
        {
            var query = _groupMemberRepository.Query();

            query = query.ApplyFilterParams(filterParams);

            var dtoQuery = _mapper.ProjectTo<GroupMemberDto>(query);

            return await dtoQuery.ToPaginatedListAsync(filterParams.PageNumber, filterParams.PageSize);
        }

        public async Task<GroupMemberDto> GetGroupMemberByIdAsync(Guid id)
        {
            var groupMemberEntity = await _groupMemberRepository.GetByIdAsync(id);
            return _mapper.Map<GroupMemberDto>(groupMemberEntity);
        }

        public async Task<ApiResponse<GroupMemberDto>> CreateGroupMemberAsync(CreateGroupMemberDto createGroupMemberDto)
        {
            var group = await _groupRepository.GetByIdAsync(createGroupMemberDto.GroupId);
            if (group == null)
            {
                return ApiResponse<GroupMemberDto>.Fail("Group not found.");
            }

            if (group.MaxMembers.HasValue)
            {
                var currentMembersCount = await _groupMemberRepository.Query()
                                                                    .Where(gm => gm.GroupId == createGroupMemberDto.GroupId)
                                                                    .CountAsync();
                if (currentMembersCount >= group.MaxMembers.Value)
                {
                    return ApiResponse<GroupMemberDto>.Fail($"Group has reached its maximum number of members ({group.MaxMembers.Value}).");
                }
            }

            var groupMemberEntity = _mapper.Map<GroupMembers>(createGroupMemberDto);
            await _groupMemberRepository.AddAsync(groupMemberEntity);
            await _groupMemberRepository.SaveChangesAsync();
            return ApiResponse<GroupMemberDto>.Ok(_mapper.Map<GroupMemberDto>(groupMemberEntity));
        }

        public async Task<bool> UpdateGroupMemberAsync(Guid id, UpdateGroupMemberDto updateGroupMemberDto)
        {
            var groupMemberEntity = await _groupMemberRepository.GetByIdAsync(id);
            if (groupMemberEntity == null)
            {
                return false;
            }

            _mapper.Map(updateGroupMemberDto, groupMemberEntity);
            _groupMemberRepository.Update(groupMemberEntity);
            await _groupMemberRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteGroupMemberAsync(Guid id)
        {
            var groupMemberEntity = await _groupMemberRepository.GetByIdAsync(id);
            if (groupMemberEntity == null)
            {
                return false;
            }

            _groupMemberRepository.Remove(groupMemberEntity);
            await _groupMemberRepository.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<GroupMemberDto>> GetGroupMembersByGroupIdAsync(Guid groupId)
        {
            var groupMembers = await _groupMemberRepository.Query()
                .Where(gm => gm.GroupId == groupId)
                .ToListAsync();

            var memberDtos = new List<GroupMemberDto>();
            foreach (var member in groupMembers)
            {
                var user = await _userManager.FindByIdAsync(member.UserId.ToString());
                if (user != null)
                {
                    memberDtos.Add(new GroupMemberDto
                    {
                        Id = member.Id,
                        GroupId = member.GroupId,
                        UserId = member.UserId,
                        UserName = user.UserName
                    });
                }
            }

            return memberDtos;
        }

        public async Task<ApiResponse<string>> KickGroupMemberAsync(Guid groupId, Guid memberId, string currentUserId)
        {
            try
            {
                var group = await _groupRepository.Query()
                                                .Include(g => g.Owner)
                                                .FirstOrDefaultAsync(g => g.Id == groupId);

                if (group == null)
                {
                    return ApiResponse<string>.Fail("Group not found.");
                }

                if (group.OwnerId != currentUserId)
                {
                    return ApiResponse<string>.Fail("Only the group owner can kick members.");
                }

                var memberToKick = await _groupMemberRepository.Query()
                    .FirstOrDefaultAsync(gm => gm.GroupId == groupId && gm.UserId == memberId);

                if (memberToKick == null)
                {
                    return ApiResponse<string>.Fail("Member not found in this group.");
                }

                if (memberToKick.UserId.ToString() == currentUserId)
                {
                    return ApiResponse<string>.Fail("Group owner cannot kick themselves.");
                }

                _groupMemberRepository.Remove(memberToKick);
                await _groupMemberRepository.SaveChangesAsync();

                await _notificationService.CreateNotificationAsync(
                    memberToKick.UserId.ToString(),
                    "You have been kicked from a group",
                    $"You have been kicked from the group '{group.GroupName}' by the owner.",
                    $"/groups"
                );

                return ApiResponse<string>.Ok("Member kicked successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.Fail($"An error occurred while kicking the member: {ex.Message}");
            }
        }
    }
}