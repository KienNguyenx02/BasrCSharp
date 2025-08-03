using AutoMapper;
using WebApplication1.Application.DTOs.GroupMembers;
using WebApplication1.Application.Interfaces;
using WebApplication1.Domain.Entities;
using Microsoft.EntityFrameworkCore; // Added this line

using WebApplication1.Shared.Results;
using WebApplication1.Shared.Extensions;
using WebApplication1.Infrastructure.Data;


namespace WebApplication1.Application.Services
{
    public class GroupMemberService : IGroupMemberService
    {
        private readonly IBaseRepository<GroupMembers> _groupMemberRepository;
        private readonly IBaseRepository<Group> _groupRepository; // New: To access group details
        private readonly IMapper _mapper;

        public GroupMemberService(IBaseRepository<GroupMembers> groupMemberRepository, IBaseRepository<Group> groupRepository, IMapper mapper)
        {
            _groupMemberRepository = groupMemberRepository;
            _groupRepository = groupRepository; // Initialize new repository
            _mapper = mapper;
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
            return _mapper.Map<IEnumerable<GroupMemberDto>>(groupMembers);
        }
    }
}