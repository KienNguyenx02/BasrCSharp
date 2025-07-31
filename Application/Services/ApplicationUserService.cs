using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Application.DTOs.Users;
using WebApplication1.Application.Interfaces;
using WebApplication1.Domain.Entities;
using WebApplication1.Shared.Results;
using WebApplication1.Shared.Extensions;

namespace WebApplication1.Application.Services
{
    public class ApplicationUserService : IApplicationUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public ApplicationUserService(UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<ApplicationUserDto>> GetAllUsersAsync(FilterParams filterParams)
        {
            var query = _userManager.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filterParams.SearchTerm))
            {
                query = query.Where(u => u.UserName.Contains(filterParams.SearchTerm) || u.Email.Contains(filterParams.SearchTerm));
            }

            query = query.ApplyFilterParams(filterParams);

            var dtoQuery = _mapper.ProjectTo<ApplicationUserDto>(query);

            return await dtoQuery.ToPaginatedListAsync(filterParams.PageNumber, filterParams.PageSize);
        }

        public async Task<ApplicationUserDto> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return _mapper.Map<ApplicationUserDto>(user);
        }

        public async Task<ApplicationUserDto> CreateUserAsync(CreateApplicationUserDto createUserDto)
        {
            var user = new ApplicationUser { UserName = createUserDto.UserName, Email = createUserDto.Email };
            var result = await _userManager.CreateAsync(user, createUserDto.Password);

            if (!result.Succeeded)
            {
                throw new Exception($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
            return _mapper.Map<ApplicationUserDto>(user);
        }

        public async Task<bool> UpdateUserAsync(UpdateApplicationUserDto updateUserDto)
        {
            var user = await _userManager.FindByIdAsync(updateUserDto.Id);
            if (user == null)
            {
                return false;
            }

            user.UserName = updateUserDto.UserName ?? user.UserName;
            user.Email = updateUserDto.Email ?? user.Email;

            if (!string.IsNullOrWhiteSpace(updateUserDto.NewPassword))
            {
                var removePasswordResult = await _userManager.RemovePasswordAsync(user);
                if (!removePasswordResult.Succeeded)
                {
                    throw new Exception($"Failed to remove old password: {string.Join(", ", removePasswordResult.Errors.Select(e => e.Description))}");
                }
                var addPasswordResult = await _userManager.AddPasswordAsync(user, updateUserDto.NewPassword);
                if (!addPasswordResult.Succeeded)
                {
                    throw new Exception($"Failed to add new password: {string.Join(", ", addPasswordResult.Errors.Select(e => e.Description))}");
                }
            }

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }
            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }
    }
}