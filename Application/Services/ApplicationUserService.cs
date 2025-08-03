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

            // Apply pagination first
            var paginatedUsers = await query.ToPaginatedListAsync(filterParams.PageNumber, filterParams.PageSize);

            var userDtos = new List<ApplicationUserDto>();
            foreach (var user in paginatedUsers.Items)
            {
                var userDto = _mapper.Map<ApplicationUserDto>(user);
                userDto.Roles = (await _userManager.GetRolesAsync(user)).ToList();
                userDtos.Add(userDto);
            }

            // Create a new PaginatedResult with the DTOs, preserving pagination metadata
            return new PaginatedResult<ApplicationUserDto>(userDtos, paginatedUsers.TotalCount, paginatedUsers.PageNumber, paginatedUsers.PageSize);
        }

        public async Task<ApplicationUserDto> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return _mapper.Map<ApplicationUserDto>(user);
        }

        public async Task<ApplicationUserDto> CreateUserAsync(CreateApplicationUserDto createUserDto)
        {
            var user = new ApplicationUser { UserName = createUserDto.UserName, Email = createUserDto.Email };
            var result = await _userManager.CreateAsync(user, "Abc@123");

            if (!result.Succeeded)
            {
                throw new Exception($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            // Assign the specified role to the new user
            if (!string.IsNullOrWhiteSpace(createUserDto.NewRole))
            {
                var roleResult = await _userManager.AddToRoleAsync(user, createUserDto.NewRole);
                if (!roleResult.Succeeded)
                {
                    // Log or handle error if role assignment fails
                    throw new Exception($"Failed to assign role {createUserDto.NewRole} to user: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                }
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

        public async Task<IEnumerable<UserLookupDto>> GetUsersForLookupAsync()
        {
            var users = await _userManager.Users
                                        .Select(u => new UserLookupDto
                                        {
                                            Id = u.Id,
                                            UserName = u.UserName,
                                            Email = u.Email
                                        })
                                        .ToListAsync();
            return users;
        }

        public async Task<bool> UpdateUserRoleAsync(UpdateUserRoleDto updateUserRoleDto)
        {
            var user = await _userManager.FindByIdAsync(updateUserRoleDto.Id);
            if (user == null)
            {
                return false;
            }

            // Update user information
            user.UserName = updateUserRoleDto.UserName ?? user.UserName;
            user.Email = updateUserRoleDto.Email ?? user.Email;

            if (!string.IsNullOrWhiteSpace(updateUserRoleDto.NewPassword))
            {
                var removePasswordResult = await _userManager.RemovePasswordAsync(user);
                if (!removePasswordResult.Succeeded)
                {
                    throw new Exception($"Failed to remove old password: {string.Join(", ", removePasswordResult.Errors.Select(e => e.Description))}");
                }
                var addPasswordResult = await _userManager.AddPasswordAsync(user, updateUserRoleDto.NewPassword);
                if (!addPasswordResult.Succeeded)
                {
                    throw new Exception($"Failed to add new password: {string.Join(", ", addPasswordResult.Errors.Select(e => e.Description))}");
                }
            }

            var updateInfoResult = await _userManager.UpdateAsync(user);
            if (!updateInfoResult.Succeeded)
            {
                throw new Exception($"Failed to update user information: {string.Join(", ", updateInfoResult.Errors.Select(e => e.Description))}");
            }

            var currentRoles = await _userManager.GetRolesAsync(user);

            // Prevent changing an admin's role if the new role is not admin
            if (currentRoles.Contains("Admin") && updateUserRoleDto.NewRole != "Admin")
            {
                return false; // Cannot demote an admin
            }

            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                throw new Exception($"Failed to remove user from roles: {string.Join(", ", removeResult.Errors.Select(e => e.Description))}");
            }

            var addResult = await _userManager.AddToRoleAsync(user, updateUserRoleDto.NewRole);
            if (!addResult.Succeeded)
            {
                throw new Exception($"Failed to add user to role: {string.Join(", ", addResult.Errors.Select(e => e.Description))}");
            }

            return true;
        }
    }
}