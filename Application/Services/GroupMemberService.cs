using AutoMapper;
using WebApplication1.Application.DTOs.GroupMembers;
using WebApplication1.Application.Interfaces;
using WebApplication1.Domain.Entities;

using WebApplication1.Shared.Results;
using WebApplication1.Shared.Extensions;
using WebApplication1.Infrastructure.Data;


namespace WebApplication1.Application.Services
{
    public class GroupMemberService : IGroupMemberService
    {
        private readonly IBaseRepository<GroupMembers> _groupMemberRepository;
        private readonly IMapper _mapper;

        public GroupMemberService(IBaseRepository<GroupMembers> groupMemberRepository, IMapper mapper)
        {
            _groupMemberRepository = groupMemberRepository;
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

        public async Task<GroupMemberDto> CreateGroupMemberAsync(CreateGroupMemberDto createGroupMemberDto)
        {
            var groupMemberEntity = _mapper.Map<GroupMembers>(createGroupMemberDto);
            await _groupMemberRepository.AddAsync(groupMemberEntity);
            await _groupMemberRepository.SaveChangesAsync();
            return _mapper.Map<GroupMemberDto>(groupMemberEntity);
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
    }
}