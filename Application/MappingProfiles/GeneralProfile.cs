using AutoMapper;
using WebApplication1.Application.DTOs.Events;
using WebApplication1.Application.DTOs.GroupMembers;
using WebApplication1.Application.DTOs.Groups;
using WebApplication1.Application.DTOs.Reminders;
using WebApplication1.Application.DTOs.UserEventStatus;
using WebApplication1.Application.DTOs.Notifications;
using WebApplication1.Application.DTOs.Users; // Added this line
using WebApplication1.Domain.Entities;

namespace WebApplication1.Application.MappingProfiles
{
    public class GeneralProfile : Profile
    {
        public GeneralProfile()
        {
            // // Product Mappings
            // CreateMap<Product, ProductDto>().ReverseMap();

            // // Category Mappings
            // CreateMap<Category, CategoryDto>().ReverseMap();
            // CreateMap<CreateCategoryDto, Category>();
            // CreateMap<UpdateCategoryDto, Category>();

            // UserProfile Mappings
            CreateMap<ApplicationUser, UserProfileDto>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName));
            CreateMap<UpdateUserProfileDto, ApplicationUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));

            // // Order Mappings
            // CreateMap<Order, OrderDto>().ReverseMap();
            // CreateMap<CreateOrderDto, Order>();
            // CreateMap<UpdateOrderDto, Order>();

            // // OrderItem Mappings
            // CreateMap<OrderItem, OrderItemDto>().ReverseMap();
            // CreateMap<CreateOrderItemDto, OrderItem>();

            // Event Mappings
            CreateMap<EventsEnitity, EventDto>().ReverseMap();
            CreateMap<CreateEventDto, EventsEnitity>();
            CreateMap<UpdateEventDto, EventsEnitity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Attendees, opt => opt.MapFrom(src => src.Attendees == null ? new List<Guid>() : src.Attendees));

            // Group Mappings
            CreateMap<GroupsEntity, GroupDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.GroupName))
                .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src => src.Permissions))
                .ForMember(dest => dest.MaxMembers, opt => opt.MapFrom(src => src.MaxMembers))
                .ReverseMap()
                .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src => src.Permissions))
                .ForMember(dest => dest.MaxMembers, opt => opt.MapFrom(src => src.MaxMembers));

            CreateMap<CreateGroupDto, GroupsEntity>()
                .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src => src.Permissions))
                .ForMember(dest => dest.MaxMembers, opt => opt.MapFrom(src => src.MaxMembers));

            CreateMap<UpdateGroupDto, GroupsEntity>()
                .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src => src.Permissions))
                .ForMember(dest => dest.MaxMembers, opt => opt.MapFrom(src => src.MaxMembers));

            // GroupMember Mappings
            CreateMap<GroupMembers, GroupMemberDto>().ReverseMap();
            CreateMap<CreateGroupMemberDto, GroupMembers>();
            CreateMap<UpdateGroupMemberDto, GroupMembers>();

            // Reminder Mappings
            CreateMap<Reminders, ReminderDto>().ReverseMap();
            CreateMap<CreateReminderDto, Reminders>()
                .ForMember(dest => dest.ReminderDate, opt => opt.MapFrom(src => src.ReminderDateTime == default(DateTime) ? DateTime.UtcNow.Date : src.ReminderDateTime.Date))
                .ForMember(dest => dest.ReminderTime, opt => opt.MapFrom(src => src.ReminderDateTime == default(DateTime) ? TimeOnly.FromDateTime(DateTime.UtcNow) : TimeOnly.FromDateTime(src.ReminderDateTime)));

            CreateMap<UpdateReminderDto, Reminders>()
                .ForMember(dest => dest.ReminderDate, opt => opt.MapFrom(src => src.ReminderDateTime == default(DateTime) ? DateTime.UtcNow.Date : src.ReminderDateTime.Date))
                .ForMember(dest => dest.ReminderTime, opt => opt.MapFrom(src => src.ReminderDateTime == default(DateTime) ? TimeOnly.FromDateTime(DateTime.UtcNow) : TimeOnly.FromDateTime(src.ReminderDateTime)));

            // UserEventStatus Mappings
            CreateMap<UserEventStatus, UserEventStatusDto>().ReverseMap();
            CreateMap<CreateUserEventStatusDto, UserEventStatus>();
            CreateMap<UpdateUserEventStatusDto, UserEventStatus>();

            // Notification Mappings
            CreateMap<Notifications, NotificationDto>().ReverseMap();
            CreateMap<CreateNotificationDto, Notifications>();
            CreateMap<UpdateNotificationDto, Notifications>();

            // ApplicationUser Mappings
            CreateMap<ApplicationUser, ApplicationUserDto>();
            CreateMap<CreateApplicationUserDto, ApplicationUser>();
            CreateMap<UpdateApplicationUserDto, ApplicationUser>();
        }
    }
}