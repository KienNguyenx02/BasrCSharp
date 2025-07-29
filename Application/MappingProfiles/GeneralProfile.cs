using AutoMapper;
using WebApplication1.Application.DTOs.Customers;
using WebApplication1.Application.DTOs.Events;
using WebApplication1.Application.DTOs.GroupMembers;
using WebApplication1.Application.DTOs.Groups;
using WebApplication1.Application.DTOs.Reminders;
using WebApplication1.Application.DTOs.UserEventStatus;
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

            // // UserProfile Mappings
            // CreateMap<ApplicationUser, UserProfileDto>().ReverseMap();
            // CreateMap<UpdateUserProfileDto, ApplicationUser>();

            // // Order Mappings
            // CreateMap<Order, OrderDto>().ReverseMap();
            // CreateMap<CreateOrderDto, Order>();
            // CreateMap<UpdateOrderDto, Order>();

            // // OrderItem Mappings
            // CreateMap<OrderItem, OrderItemDto>().ReverseMap();
            // CreateMap<CreateOrderItemDto, OrderItem>();

            // // Customer Mappings
            CreateMap<Customer, CustomerDto>().ReverseMap();
            CreateMap<CreateCustomerDto, Customer>();
            CreateMap<UpdateCustomerDto, Customer>();

            // Event Mappings
            CreateMap<EventsEnitity, EventDto>().ReverseMap();
            CreateMap<CreateEventDto, EventsEnitity>()
                .ForMember(dest => dest.EventDate, opt => opt.MapFrom(src => src.StartDate == default(DateTime) ? DateTime.UtcNow : src.StartDate))
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => TimeOnly.FromDateTime(src.StartDate)))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => TimeOnly.FromDateTime(src.EndDate)));

            CreateMap<UpdateEventDto, EventsEnitity>()
                .ForMember(dest => dest.EventDate, opt => opt.MapFrom(src => src.StartDate == default(DateTime) ? DateTime.UtcNow : src.StartDate))
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => TimeOnly.FromDateTime(src.StartDate)))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => TimeOnly.FromDateTime(src.EndDate)));

            // Group Mappings
            CreateMap<GroupsEntity, GroupDto>().ReverseMap();
            CreateMap<CreateGroupDto, GroupsEntity>();
            CreateMap<UpdateGroupDto, GroupsEntity>();

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
        }
    }
}