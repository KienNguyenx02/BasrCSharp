using System.Threading.Tasks;
using WebApplication1.Application.DTOs.Users;

namespace WebApplication1.Application.Interfaces
{
    public interface IUserProfileService
    {
        Task<UserProfileDto> GetUserProfileAsync(string userName);
        Task<bool> UpdateUserProfileAsync(string userName, UpdateUserProfileDto updateUserProfileDto);
    }
}