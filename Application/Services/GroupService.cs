using AutoMapper;
using WebApplication1.Application.DTOs.Groups;
using WebApplication1.Application.Interfaces;
using WebApplication1.Domain.Entities;
using WebApplication1.Infrastructure.Data;
using WebApplication1.Shared.Results;
using WebApplication1.Shared.Extensions;

namespace WebApplication1.Application.Services
{
    public class GroupService : IGroupService
    {
        private readonly IBaseRepository<Group> _groupRepository;
        private readonly IMapper _mapper;

        public GroupService(IBaseRepository<Group> groupRepository, IMapper mapper)
        {
            _groupRepository = groupRepository;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<GroupDto>> GetGroupsAsync(FilterParams filterParams)
        {
            var query = _groupRepository.Query();

            query = query.ApplyFilterParams(filterParams);

            var dtoQuery = _mapper.ProjectTo<GroupDto>(query);

            return await dtoQuery.ToPaginatedListAsync(filterParams.PageNumber, filterParams.PageSize);
        }

        public async Task<GroupDto> GetGroupByIdAsync(Guid id)
        {
            var groupEntity = await _groupRepository.GetByIdAsync(id);
            return _mapper.Map<GroupDto>(groupEntity);
        }

        public async Task<GroupDto> CreateGroupAsync(CreateGroupDto createGroupDto, string ownerId)
        {
            var groupEntity = _mapper.Map<Group>(createGroupDto);
            groupEntity.OwnerId = ownerId;
            await _groupRepository.AddAsync(groupEntity);
            await _groupRepository.SaveChangesAsync();
            return _mapper.Map<GroupDto>(groupEntity);
        }

        public async Task<bool> UpdateGroupAsync(Guid id, UpdateGroupDto updateGroupDto)
        {
            var groupEntity = await _groupRepository.GetByIdAsync(id);
            if (groupEntity == null)
            {
                return false;
            }

            _mapper.Map(updateGroupDto, groupEntity);
            _groupRepository.Update(groupEntity);
            await _groupRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteGroupAsync(Guid id)
        {
            var groupEntity = await _groupRepository.GetByIdAsync(id);
            if (groupEntity == null)
            {
                return false;
            }

            _groupRepository.Remove(groupEntity);
            await _groupRepository.SaveChangesAsync();
            return true;
        }

        public async Task<PaginatedResult<GroupDto>> GetGroupsByOwnerIdAsync(string ownerId, FilterParams filterParams)
        {
            var query = _groupRepository.Query()
                                        .Where(g => g.OwnerId == ownerId);

            query = query.ApplyFilterParams(filterParams);

            var dtoQuery = _mapper.ProjectTo<GroupDto>(query);

            return await dtoQuery.ToPaginatedListAsync(filterParams.PageNumber, filterParams.PageSize);
        }
    }
}