using AutoMapper;
using WebApplication1.Application.DTOs.Reminders;
using WebApplication1.Application.Interfaces;
using WebApplication1.Domain.Entities;

using WebApplication1.Shared.Results;
using WebApplication1.Shared.Extensions;
using WebApplication1.Infrastructure.Data;

namespace WebApplication1.Application.Services
{
    public class ReminderService : IReminderService
    {
        private readonly IBaseRepository<Reminders> _reminderRepository;
        private readonly IMapper _mapper;

        public ReminderService(IBaseRepository<Reminders> reminderRepository, IMapper mapper)
        {
            _reminderRepository = reminderRepository;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<ReminderDto>> GetRemindersAsync(FilterParams filterParams)
        {
            var query = _reminderRepository.Query();

            query = query.ApplyFilterParams(filterParams);

            var dtoQuery = _mapper.ProjectTo<ReminderDto>(query);

            return await dtoQuery.ToPaginatedListAsync(filterParams.PageNumber, filterParams.PageSize);
        }

        public async Task<ReminderDto> GetReminderByIdAsync(Guid id)
        {
            var reminderEntity = await _reminderRepository.GetByIdAsync(id);
            return _mapper.Map<ReminderDto>(reminderEntity);
        }

        public async Task<ReminderDto> CreateReminderAsync(CreateReminderDto createReminderDto)
        {
            var reminderEntity = _mapper.Map<Reminders>(createReminderDto);
            await _reminderRepository.AddAsync(reminderEntity);
            await _reminderRepository.SaveChangesAsync();
            return _mapper.Map<ReminderDto>(reminderEntity);
        }

        public async Task<bool> UpdateReminderAsync(Guid id, UpdateReminderDto updateReminderDto)
        {
            var reminderEntity = await _reminderRepository.GetByIdAsync(id);
            if (reminderEntity == null)
            {
                return false;
            }

            _mapper.Map(updateReminderDto, reminderEntity);
            _reminderRepository.Update(reminderEntity);
            await _reminderRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteReminderAsync(Guid id)
        {
            var reminderEntity = await _reminderRepository.GetByIdAsync(id);
            if (reminderEntity == null)
            {
                return false;
            }

            _reminderRepository.Remove(reminderEntity);
            await _reminderRepository.SaveChangesAsync();
            return true;
        }
    }
}