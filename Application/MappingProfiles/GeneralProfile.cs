using AutoMapper;
using WebApplication1.Application.DTOs.Events;
using WebApplication1.Application.DTOs.GroupMembers;
using WebApplication1.Application.DTOs.Groups;
using WebApplication1.Application.DTOs.Reminders;
using WebApplication1.Application.DTOs.UserEventStatus;
using WebApplication1.Application.DTOs.Notifications;
using WebApplication1.Application.DTOs.Users; // Added this line
using WebApplication1.Domain.Entities;
using WebApplication1.Application.DTOs.GroupInvitations;
using WebApplication1.Application.DTOs.GroupJoinRequest;

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
            CreateMap<Group, GroupDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.GroupName))
                .ReverseMap();

            CreateMap<CreateGroupDto, Group>()
                .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Name));

            CreateMap<UpdateGroupDto, Group>()
                .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Name));

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

            // GroupInvitation Mappings
            CreateMap<GroupInvitation, GroupInvitationDto>()
                .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Group.GroupName))
                .ForMember(dest => dest.InviterUsername, opt => opt.MapFrom(src => src.Inviter.UserName))
                .ForMember(dest => dest.InvitedUserUsername, opt => opt.MapFrom(src => src.InvitedUser.UserName));

            // ApplicationUser Mappings
            CreateMap<ApplicationUser, ApplicationUserDto>();
            CreateMap<CreateApplicationUserDto, ApplicationUser>();
            CreateMap<UpdateApplicationUserDto, ApplicationUser>();
            CreateMap<ApplicationUser, UserLookupDto>();

            // GroupJoinRequest Mappings
            CreateMap<GroupJoinRequest, GroupJoinRequestDto>()
                .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Group.GroupName))
                .ForMember(dest => dest.RequestingUserName, opt => opt.MapFrom(src => src.RequestingUser.UserName))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
        }
    }
}
