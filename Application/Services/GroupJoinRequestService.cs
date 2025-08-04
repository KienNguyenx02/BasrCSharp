using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Application.DTOs.GroupJoinRequest;
using WebApplication1.Application.Interfaces;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Enums;
using WebApplication1.Infrastructure.Data;
using WebApplication1.Shared.Results;
using WebApplication1.Shared.Extensions;

namespace WebApplication1.Application.Services
{
    public class GroupJoinRequestService : IGroupJoinRequestService
    {
        private readonly IBaseRepository<GroupJoinRequest> _joinRequestRepository;
        private readonly IBaseRepository<Group> _groupRepository;
        private readonly IBaseRepository<GroupMembers> _groupMemberRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;

        public GroupJoinRequestService(
            IBaseRepository<GroupJoinRequest> joinRequestRepository,
            IBaseRepository<Group> groupRepository,
            IBaseRepository<GroupMembers> groupMemberRepository,
            UserManager<ApplicationUser> userManager,
            INotificationService notificationService,
            IMapper mapper)
        {
            _joinRequestRepository = joinRequestRepository;
            _groupRepository = groupRepository;
            _groupMemberRepository = groupMemberRepository;
            _userManager = userManager;
            _notificationService = notificationService;
            _mapper = mapper;
        }

        public async Task<ApiResponse<GroupJoinRequestDto>> CreateGroupJoinRequestAsync(CreateGroupJoinRequestDto dto, string requestingUserId)
        {
            var group = await _groupRepository.GetByIdAsync(dto.GroupId);
            if (group == null)
            {
                return ApiResponse<GroupJoinRequestDto>.Fail("Group not found.");
            }

            var isMember = await _groupMemberRepository.Query()
                .AnyAsync(gm => gm.GroupId == dto.GroupId && gm.UserId.ToString() == requestingUserId);
            if (isMember)
            {
                return ApiResponse<GroupJoinRequestDto>.Fail("You are already a member of this group.");
            }

            var existingRequest = await _joinRequestRepository.Query()
                .AnyAsync(req => req.GroupId == dto.GroupId && req.RequestingUserId == requestingUserId && req.Status == JoinRequestStatus.Pending);
            if (existingRequest)
            {
                return ApiResponse<GroupJoinRequestDto>.Fail("You have already sent a request to join this group.");
            }

            var joinRequest = new GroupJoinRequest
            {
                GroupId = dto.GroupId,
                RequestingUserId = requestingUserId
            };

            await _joinRequestRepository.AddAsync(joinRequest);
            await _joinRequestRepository.SaveChangesAsync();

            var groupOwner = await _userManager.FindByIdAsync(group.OwnerId);
            if (groupOwner == null)
            {
                return ApiResponse<GroupJoinRequestDto>.Fail("Group owner not found.");
            }
            await _notificationService.CreateNotificationAsync(
                group.OwnerId,
                "New Join Request",
                $"You have a new request to join your group '{group.GroupName}' from {groupOwner.UserName}.",
                $"/groups/requests/{group.Id}"
            );

            return ApiResponse<GroupJoinRequestDto>.Ok(_mapper.Map<GroupJoinRequestDto>(joinRequest));
        }

        public async Task<ApiResponse<string>> RespondToGroupJoinRequestAsync(Guid requestId, bool accept, string currentUserId)
        {
            var joinRequest = await _joinRequestRepository.Query()
                .Include(req => req.Group)
                .Include(req => req.RequestingUser)
                .FirstOrDefaultAsync(req => req.Id == requestId);

            if (joinRequest == null)
            {
                return ApiResponse<string>.Fail("Join request not found.");
            }

            if (joinRequest.Group.OwnerId != currentUserId)
            {
                return ApiResponse<string>.Fail("Only the group owner can respond to join requests.");
            }

            if (joinRequest.Status != JoinRequestStatus.Pending)
            {
                return ApiResponse<string>.Fail("This join request has already been responded to.");
            }

            if (accept)
            {
                joinRequest.Status = JoinRequestStatus.Accepted;

                var groupMember = new GroupMembers
                {
                    GroupId = joinRequest.GroupId,
                    UserId = Guid.Parse(joinRequest.RequestingUserId),
                    GroupRole = GroupRole.Member
                };
                await _groupMemberRepository.AddAsync(groupMember);

                await _notificationService.CreateNotificationAsync(
                    joinRequest.RequestingUserId,
                    "Group Join Request Accepted",
                    $"Your request to join the group '{joinRequest.Group.GroupName}' has been accepted.",
                    $"/groups/{joinRequest.GroupId}"
                );
            }
            else
            {
                joinRequest.Status = JoinRequestStatus.Declined;

                await _notificationService.CreateNotificationAsync(
                    joinRequest.RequestingUserId,
                    "Group Join Request Declined",
                    $"Your request to join the group '{joinRequest.Group.GroupName}' has been declined.",
                    $"/groups"
                );
            }

            _joinRequestRepository.Update(joinRequest);
            await _joinRequestRepository.SaveChangesAsync();

            return ApiResponse<string>.Ok(accept ? "Request accepted." : "Request declined.");
        }

        public async Task<PaginatedResult<GroupJoinRequestDto>> GetPendingJoinRequestsForGroupAsync(Guid groupId, FilterParams filterParams, string currentUserId)
        {
            var group = await _groupRepository.GetByIdAsync(groupId);
            if (group == null || group.OwnerId != currentUserId)
            {
                return new PaginatedResult<GroupJoinRequestDto>(new List<GroupJoinRequestDto>(), 0, filterParams.PageNumber, filterParams.PageSize);
            }

            var query = _joinRequestRepository.Query()
                .Where(req => req.GroupId == groupId && req.Status == JoinRequestStatus.Pending)
                .Include(req => req.RequestingUser)
                .Include(req => req.Group);

            var dtoQuery = _mapper.ProjectTo<GroupJoinRequestDto>(query);

            return await dtoQuery.ToPaginatedListAsync(filterParams.PageNumber, filterParams.PageSize);
        }

        public async Task<int> GetPendingJoinRequestCountForOwnerAsync(string ownerId)
        {
            var count = await _groupRepository.Query()
                .Where(g => g.OwnerId == ownerId)
                .SelectMany(g => g.GroupJoinRequests)
                .CountAsync(req => req.Status == JoinRequestStatus.Pending);

            return count;
        }

        public async Task<PaginatedResult<GroupJoinRequestDto>> GetAllPendingJoinRequestsForOwnerAsync(FilterParams filterParams, string ownerId)
        {
            var query = _joinRequestRepository.Query()
                .Include(req => req.Group)
                .Include(req => req.RequestingUser)
                .Where(req => req.Status == JoinRequestStatus.Pending && req.Group.OwnerId == ownerId);

            var dtoQuery = _mapper.ProjectTo<GroupJoinRequestDto>(query);

            return await dtoQuery.ToPaginatedListAsync(filterParams.PageNumber, filterParams.PageSize);
        }
    }
}
