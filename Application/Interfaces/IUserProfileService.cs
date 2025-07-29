using System.Threading.Tasks;
using WebApplication1.Application.DTOs.Users;

namespace WebApplication1.Application.Interfaces
{
    public interface IUserProfileService
    {
        Task<UserProfileDto> GetUserProfileAsync(string userId);
        Task<bool> UpdateUserProfileAsync(string userId, UpdateUserProfileDto updateUserProfileDto);
    }
}