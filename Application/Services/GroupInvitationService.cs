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
            var group = await _groupRepository.GetByIdAsync(dto.GroupId);
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

            // Check if there's a pending invitation already
            var existingInvitation = await _invitationRepository.Query()
                                                                .AnyAsync(inv => inv.GroupId == dto.GroupId && inv.InvitedUserId == invitedUser.Id && inv.Status == InvitationStatus.Pending);
            if (existingInvitation)
            {
                return ApiResponse<GroupInvitationDto>.Fail("A pending invitation already exists for this user and group.");
            }

            var invitation = new GroupInvitation
            {
                GroupId = dto.GroupId,
                InviterId = inviterId,
                InvitedUserId = invitedUser.Id,
                Status = InvitationStatus.Pending,
                DateSent = DateTime.UtcNow
            };

            await _invitationRepository.AddAsync(invitation);
            await _invitationRepository.SaveChangesAsync();

            // Send notification to the invited user
            await _notificationService.CreateNotificationAsync(
                invitedUser.Id,
                "New Group Invitation",
                $"You have been invited to join the group '{group.GroupName}' by {group.Owner.UserName}.",
                $"/invitations/{invitation.Id}" // Link to respond to the invitation
            );

            return ApiResponse<GroupInvitationDto>.Ok(_mapper.Map<GroupInvitationDto>(invitation));
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

            if (dto.Accept)
            {
                // Add user to group members
                var groupMember = new GroupMembers
                {
                    GroupId = invitation.GroupId,
                    UserId = Guid.Parse(invitedUserId), // Convert string to Guid
                    GroupRole = GroupRole.Member // Default role for new members
                };
                await _groupMemberRepository.AddAsync(groupMember);
                invitation.Status = InvitationStatus.Accepted;

                // Notify inviter that the invitation was accepted
                await _notificationService.CreateNotificationAsync(
                    invitation.InviterId,
                    "Group Invitation Accepted",
                    $"{invitation.InvitedUser.UserName} has accepted your invitation to join '{invitation.Group.GroupName}'."
                );
            }
            else
            {
                invitation.Status = InvitationStatus.Declined;

                // Notify inviter that the invitation was declined
                await _notificationService.CreateNotificationAsync(
                    invitation.InviterId,
                    "Group Invitation Declined",
                    $"{invitation.InvitedUser.UserName} has declined your invitation to join '{invitation.Group.GroupName}'."
                );
            }

            _invitationRepository.Update(invitation);
            await _invitationRepository.SaveChangesAsync();
            await _groupMemberRepository.SaveChangesAsync(); // Save changes for group member if accepted

            return ApiResponse<string>.Ok(dto.Accept ? "Invitation accepted." : "Invitation declined.");
        }

        public async Task<PaginatedResult<GroupInvitationDto>> GetPendingInvitationsForUserAsync(string userId, FilterParams filterParams)
        {
            var query = _invitationRepository.Query()
                                            .Where(inv => inv.InvitedUserId == userId && inv.Status == InvitationStatus.Pending)
                                            .Include(inv => inv.Group)
                                            .Include(inv => inv.Inviter);

            var dtoQuery = _mapper.ProjectTo<GroupInvitationDto>(query);

            return await dtoQuery.ToPaginatedListAsync(filterParams.PageNumber, filterParams.PageSize);
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