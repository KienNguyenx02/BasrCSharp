using AutoMapper;
using WebApplication1.Application.DTOs.UserEventStatus;
using WebApplication1.Application.Interfaces;
using WebApplication1.Domain.Entities;
using WebApplication1.Infrastructure.Data;

using WebApplication1.Shared.Results;
using WebApplication1.Shared.Extensions;

namespace WebApplication1.Application.Services
{
    public class UserEventStatusService : IUserEventStatusService
    {
        private readonly IBaseRepository<UserEventStatus> _userEventStatusRepository;
        private readonly IMapper _mapper;

        public UserEventStatusService(IBaseRepository<UserEventStatus> userEventStatusRepository, IMapper mapper)
        {
            _userEventStatusRepository = userEventStatusRepository;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<UserEventStatusDto>> GetUserEventStatusesAsync(FilterParams filterParams)
        {
            var query = _userEventStatusRepository.Query();

            query = query.ApplyFilterParams(filterParams);

            var dtoQuery = _mapper.ProjectTo<UserEventStatusDto>(query);

            return await dtoQuery.ToPaginatedListAsync(filterParams.PageNumber, filterParams.PageSize);
        }

        public async Task<UserEventStatusDto> GetUserEventStatusByIdAsync(Guid id)
        {
            var userEventStatusEntity = await _userEventStatusRepository.GetByIdAsync(id);
            return _mapper.Map<UserEventStatusDto>(userEventStatusEntity);
        }

        public async Task<UserEventStatusDto> CreateUserEventStatusAsync(CreateUserEventStatusDto createUserEventStatusDto)
        {
            var userEventStatusEntity = _mapper.Map<UserEventStatus>(createUserEventStatusDto);
            await _userEventStatusRepository.AddAsync(userEventStatusEntity);
            await _userEventStatusRepository.SaveChangesAsync();
            return _mapper.Map<UserEventStatusDto>(userEventStatusEntity);
        }

        public async Task<bool> UpdateUserEventStatusAsync(Guid id, UpdateUserEventStatusDto updateUserEventStatusDto)
        {
            var userEventStatusEntity = await _userEventStatusRepository.GetByIdAsync(id);
            if (userEventStatusEntity == null)
            {
                return false;
            }

            _mapper.Map(updateUserEventStatusDto, userEventStatusEntity);
            _userEventStatusRepository.Update(userEventStatusEntity);
            await _userEventStatusRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUserEventStatusAsync(Guid id)
        {
            var userEventStatusEntity = await _userEventStatusRepository.GetByIdAsync(id);
            if (userEventStatusEntity == null)
            {
                return false;
            }

            _userEventStatusRepository.Remove(userEventStatusEntity);
            await _userEventStatusRepository.SaveChangesAsync();
            return true;
        }
    }
}