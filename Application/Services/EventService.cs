using AutoMapper;
using WebApplication1.Application.DTOs.Events;
using WebApplication1.Application.Interfaces;
using WebApplication1.Domain.Entities;

using WebApplication1.Shared.Results;
using WebApplication1.Shared.Extensions;
using WebApplication1.Infrastructure.Data;


namespace WebApplication1.Application.Services
{
    public class EventService : IEventService
    {
        private readonly IBaseRepository<EventsEnitity> _eventRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Service for managing events
        /// </summary>
        /// <param name="eventRepository">Repository for events</param>
        /// <param name="mapper">Mapper for mapping between entity and DTO</param>
        public EventService(IBaseRepository<EventsEnitity> eventRepository, IMapper mapper)
        {
            _eventRepository = eventRepository;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<EventDto>> GetEventsAsync(FilterParams filterParams)
        {
            var query = _eventRepository.Query();


            query = query.ApplyFilterParams(filterParams);

            var dtoQuery = _mapper.ProjectTo<EventDto>(query);

            return await dtoQuery.ToPaginatedListAsync(filterParams.PageNumber, filterParams.PageSize);
        }

        public async Task<EventDto> GetEventByIdAsync(Guid id)
        {
            var eventEntity = await _eventRepository.GetByIdAsync(id);
            return _mapper.Map<EventDto>(eventEntity);
        }

        public async Task<EventDto> CreateEventAsync(CreateEventDto createEventDto)
        {
            var eventEntity = _mapper.Map<EventsEnitity>(createEventDto);
            await _eventRepository.AddAsync(eventEntity);
            await _eventRepository.SaveChangesAsync();
            return _mapper.Map<EventDto>(eventEntity);
        }

        public async Task<bool> UpdateEventAsync(Guid id, UpdateEventDto updateEventDto)
        {
            var eventEntity = await _eventRepository.GetByIdAsync(id);
            if (eventEntity == null)
            {
                return false;
            }

            _mapper.Map(updateEventDto, eventEntity);
            _eventRepository.Update(eventEntity);
            await _eventRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteEventAsync(Guid id)
        {
            var eventEntity = await _eventRepository.GetByIdAsync(id);
            if (eventEntity == null)
            {
                return false;
            }

            _eventRepository.Remove(eventEntity);
            await _eventRepository.SaveChangesAsync();
            return true;
        }
    }
}