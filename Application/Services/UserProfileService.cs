using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using WebApplication1.Application.DTOs.Users;
using WebApplication1.Application.Interfaces;
using WebApplication1.Domain.Entities;

namespace WebApplication1.Application.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public UserProfileService(UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<UserProfileDto> GetUserProfileAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return null;
            }

            return _mapper.Map<UserProfileDto>(user);
        }

        public async Task<bool> UpdateUserProfileAsync(string userName, UpdateUserProfileDto updateUserProfileDto)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return false;
            }

            _mapper.Map(updateUserProfileDto, user);

            // If password fields are provided, attempt to change password
            if (!string.IsNullOrEmpty(updateUserProfileDto.NewPassword) && !string.IsNullOrEmpty(updateUserProfileDto.CurrentPassword))
            {
                var changePasswordResult = await _userManager.ChangePasswordAsync(user, updateUserProfileDto.CurrentPassword, updateUserProfileDto.NewPassword);
                if (!changePasswordResult.Succeeded)
                {
                    // Handle password change errors
                    return false;
                }
            }

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }
    }
}