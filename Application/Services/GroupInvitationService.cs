using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Application.DTOs.GroupInvitations;
using WebApplication1.Application.Interfaces;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Enums;
using WebApplication1.Infrastructure.Data;
using WebApplication1.Shared.Results;
using WebApplication1.Shared.Extensions;

namespace WebApplication1.Application.Services
{
    public class GroupInvitationService : IGroupInvitationService
    {
        private readonly IBaseRepository<GroupInvitation> _invitationRepository;
        private readonly IBaseRepository<Group> _groupRepository;
        private readonly IBaseRepository<GroupMembers> _groupMemberRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;

        public GroupInvitationService(
            IBaseRepository<GroupInvitation> invitationRepository,
            IBaseRepository<Group> groupRepository,
            IBaseRepository<GroupMembers> groupMemberRepository,
            UserManager<ApplicationUser> userManager,
            INotificationService notificationService,
            IMapper mapper)
        {
            _invitationRepository = invitationRepository;
            _groupRepository = groupRepository;
            _groupMemberRepository = groupMemberRepository;
            _userManager = userManager;
            _notificationService = notificationService;
            _mapper = mapper;
        }

        public async Task<ApiResponse<GroupInvitationDto>> CreateGroupInvitationAsync(CreateGroupInvitationDto dto, string inviterId)
        {
            try
            {
                var group = await _groupRepository.Query().Include(g => g.Owner).FirstOrDefaultAsync(g => g.Id == dto.GroupId);
            if (group == null)
            {
                return ApiResponse<GroupInvitationDto>.Fail("Group not found.");
            }

            var invitedUser = await _userManager.FindByEmailAsync(dto.InvitedUserEmail) ?? await _userManager.FindByNameAsync(dto.InvitedUserEmail);
            if (invitedUser == null)
            {
                return ApiResponse<GroupInvitationDto>.Fail("Invited user not found.");
            }

            // Check if user is already a member of the group
            var isMember = await _groupMemberRepository.Query()
                                                        .AnyAsync(gm => gm.GroupId == dto.GroupId && gm.UserId.ToString() == invitedUser.Id);
            if (isMember)
            {
                return ApiResponse<GroupInvitationDto>.Fail("User is already a member of this group.");
            }

            // Directly add user to group members
            var groupMember = new GroupMembers
            {
                GroupId = dto.GroupId,
                UserId = Guid.Parse(invitedUser.Id),
                GroupRole = GroupRole.Member // Default role for new members
            };
            await _groupMemberRepository.AddAsync(groupMember);
            await _groupMemberRepository.SaveChangesAsync();

            // Create an invitation record with Accepted status
            var invitation = new GroupInvitation
            {
                GroupId = dto.GroupId,
                InviterId = inviterId,
                InvitedUserId = invitedUser.Id,
                Status = InvitationStatus.Pending, // Automatically accepted
                DateSent = DateTime.UtcNow
            };

            await _invitationRepository.AddAsync(invitation);
            await _invitationRepository.SaveChangesAsync();

            // Send notification to the invited user that they have been added
            await _notificationService.CreateNotificationAsync(
                invitedUser.Id,
                "Group Membership Confirmation",
                $"You have been automatically added to the group '{group.GroupName}' by {group.Owner.UserName}.",
                $"/groups/{group.Id}" // Link to the group
            );

            return ApiResponse<GroupInvitationDto>.Ok(_mapper.Map<GroupInvitationDto>(invitation));
            }
            
            catch (Exception ex)
            {
                return ApiResponse<GroupInvitationDto>.Fail($"An error occurred while creating the invitation: {ex.Message}");
            }
            
        }

        public async Task<ApiResponse<string>> RespondToGroupInvitationAsync(Guid invitationId, string invitedUserId, RespondGroupInvitationDto dto)
        {
            var invitation = await _invitationRepository.Query()
                                                        .Include(inv => inv.Group)
                                                        .Include(inv => inv.Inviter)
                                                        .FirstOrDefaultAsync(inv => inv.Id == invitationId && inv.InvitedUserId == invitedUserId && inv.Status == InvitationStatus.Pending);

            if (invitation == null)
            {
                return ApiResponse<string>.Fail("Invitation not found or already responded to.");
            }

            if (invitation.Status == InvitationStatus.Accepted)
            {
                return ApiResponse<string>.Fail("Invitation has already been accepted.");
            }

            if (!dto.Accept)
            {
                invitation.Status = InvitationStatus.Declined;

                // Notify inviter that the invitation was declined
                await _notificationService.CreateNotificationAsync(
                    invitation.InviterId,
                    "Group Invitation Declined",
                    $"{invitation.InvitedUser.UserName} has declined your invitation to join '{invitation.Group.GroupName}'."
                );
            }
            else
            {
                return ApiResponse<string>.Fail("Invitations are now automatically accepted upon creation. This endpoint is only for declining.");
            }

            _invitationRepository.Update(invitation);
            await _invitationRepository.SaveChangesAsync();

            return ApiResponse<string>.Ok("Invitation declined.");
        }

        public async Task<PaginatedResult<GroupInvitationDto>> GetPendingInvitationsForUserAsync(string userId, FilterParams filterParams)
        {
            // Since invitations are now automatically accepted, there are no pending invitations.
            // This method can be removed if not needed, or repurposed to get accepted invitations.
            return await Task.FromResult(new PaginatedResult<GroupInvitationDto>(new List<GroupInvitationDto>(), 0, filterParams.PageNumber, filterParams.PageSize));
        }

        public async Task<PaginatedResult<GroupInvitationDto>> GetSentInvitationsByInviterAsync(string inviterId, FilterParams filterParams)
        {
            var query = _invitationRepository.Query()
                                            .Where(inv => inv.InviterId == inviterId)
                                            .Include(inv => inv.Group)
                                            .Include(inv => inv.InvitedUser);

            var dtoQuery = _mapper.ProjectTo<GroupInvitationDto>(query);

            return await dtoQuery.ToPaginatedListAsync(filterParams.PageNumber, filterParams.PageSize);
        }

        public async Task<Domain.Entities.GroupInvitation> GetInvitationByIdAsync(Guid invitationId)
        {
            return await _invitationRepository.GetByIdAsync(invitationId);
        }
    }
}