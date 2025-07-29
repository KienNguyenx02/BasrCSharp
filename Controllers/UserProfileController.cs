using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApplication1.Application.DTOs.Users;
using WebApplication1.Application.Interfaces;
using WebApplication1.Shared.ErrorCodes;
using WebApplication1.Shared.Results;

namespace WebApplication1.Controllers
{
    // [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;

        public UserProfileController(IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Or ClaimTypes.Name for Username
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<object>.Fail("User not authenticated."));
            }

            var userProfile = await _userProfileService.GetUserProfileAsync(userId);
            if (userProfile == null)
            {
                return NotFound(ApiResponse<object>.Fail($"User profile not found.{ErrorCode.NotFound}"));
            }

            return Ok(ApiResponse<object>.Ok(userProfile));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUserProfile([FromBody] UpdateUserProfileDto updateUserProfileDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Or ClaimTypes.Name for Username
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<object>.Fail("User not authenticated."));
            }

            var result = await _userProfileService.UpdateUserProfileAsync(userId, updateUserProfileDto);
            if (!result)
            {
                return BadRequest(ApiResponse<object>.Fail($"Failed to update user profile.{ErrorCode.ValidationError}"));
            }

            return Ok(ApiResponse<string>.Ok("User profile updated successfully."));
        }
    }
}