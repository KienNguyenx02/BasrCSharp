using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Application.DTOs.Notifications;
using WebApplication1.Application.Interfaces;
using WebApplication1.Domain.Entities;
using WebApplication1.Infrastructure.Data;
using WebApplication1.Shared.Extensions;
using WebApplication1.Shared.Results;

namespace WebApplication1.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IBaseRepository<Notifications> _notificationRepository;
        private readonly IMapper _mapper;

        public NotificationService(IBaseRepository<Notifications> notificationRepository, IMapper mapper)
        {
            _notificationRepository = notificationRepository;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<NotificationDto>> GetNotificationsAsync(FilterParams filterParams)
        {
            var query = _notificationRepository.Query();

            if (!string.IsNullOrWhiteSpace(filterParams.SearchTerm))
            {
                query = query.Where(n => n.Message.Contains(filterParams.SearchTerm));
            }

            query = query.ApplyFilterParams(filterParams);

            var dtoQuery = _mapper.ProjectTo<NotificationDto>(query);

            return await dtoQuery.ToPaginatedListAsync(filterParams.PageNumber, filterParams.PageSize);
        }

        public async Task<NotificationDto> GetNotificationByIdAsync(Guid id)
        {
            var notification = await _notificationRepository.GetByIdAsync(id);
            return _mapper.Map<NotificationDto>(notification);
        }

        public async Task<NotificationDto> CreateNotificationAsync(CreateNotificationDto createNotificationDto)
        {
            var notification = _mapper.Map<Notifications>(createNotificationDto);
            notification.CreatedAt = DateTime.UtcNow;
            notification.IsRead = false;

            await _notificationRepository.AddAsync(notification);
            await _notificationRepository.SaveChangesAsync();
            return _mapper.Map<NotificationDto>(notification);
        }

        public async Task<bool> UpdateNotificationAsync(Guid id, UpdateNotificationDto updateNotificationDto)
        {
            var notification = await _notificationRepository.GetByIdAsync(id);
            if (notification == null)
            {
                return false;
            }

            _mapper.Map(updateNotificationDto, notification);
            _notificationRepository.Update(notification);
            await _notificationRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteNotificationAsync(Guid id)
        {
            var notification = await _notificationRepository.GetByIdAsync(id);
            if (notification == null)
            {
                return false;
            }

            _notificationRepository.Remove(notification);
            await _notificationRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkNotificationAsReadAsync(Guid id)
        {
            var notification = await _notificationRepository.GetByIdAsync(id);
            if (notification == null)
            {
                return false;
            }
            notification.IsRead = true;
            _notificationRepository.Update(notification);
            await _notificationRepository.SaveChangesAsync();
            return true;
        }

        public async Task CreateNotificationAsync(string userId, string title, string message, string? link = null)
        {
            var notification = new Notifications
            {
                UserId = userId,
                Title = title,
                Message = message,
                Link = link,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };
            await _notificationRepository.AddAsync(notification);
            await _notificationRepository.SaveChangesAsync();
        }
    }
}