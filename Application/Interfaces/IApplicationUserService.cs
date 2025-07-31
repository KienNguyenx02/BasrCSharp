using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.Application.DTOs.Users;
using WebApplication1.Shared.Results;

namespace WebApplication1.Application.Interfaces
{
    public interface IApplicationUserService
    {
        Task<PaginatedResult<ApplicationUserDto>> GetAllUsersAsync(FilterParams filterParams);
        Task<ApplicationUserDto> GetUserByIdAsync(string userId);
        Task<ApplicationUserDto> CreateUserAsync(CreateApplicationUserDto createUserDto);
        Task<bool> UpdateUserAsync(UpdateApplicationUserDto updateUserDto);
        Task<bool> DeleteUserAsync(string userId);
    }
}