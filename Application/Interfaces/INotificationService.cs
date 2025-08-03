using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.Application.DTOs.Notifications;
using WebApplication1.Shared.Results;

namespace WebApplication1.Application.Interfaces
{
    public interface INotificationService
    {
        Task<PaginatedResult<NotificationDto>> GetNotificationsAsync(FilterParams filterParams);
        Task<NotificationDto> GetNotificationByIdAsync(Guid id);
        Task<NotificationDto> CreateNotificationAsync(CreateNotificationDto createNotificationDto);
        Task<bool> UpdateNotificationAsync(Guid id, UpdateNotificationDto updateNotificationDto);
        Task<bool> DeleteNotificationAsync(Guid id);
        Task<bool> MarkNotificationAsReadAsync(Guid id);
        Task CreateNotificationAsync(string userId, string title, string message, string? link = null);
    }
}