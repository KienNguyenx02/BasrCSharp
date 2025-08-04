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
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;

        public UserProfileController(IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetUserProfile()
        {
            var userName = User.FindFirstValue(ClaimTypes.Name); // Changed to ClaimTypes.Name
            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized(ApiResponse<object>.Fail("User not authenticated."));
            }

            var userProfile = await _userProfileService.GetUserProfileAsync(userName); // Pass userName
            if (userProfile == null)
            {
                return NotFound(ApiResponse<object>.Fail($"User profile not found.{ErrorCode.NotFound("User").Message}"));
            }

            return Ok(ApiResponse<object>.Ok(userProfile));
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateUserProfile([FromBody] UpdateUserProfileDto updateUserProfileDto)
        {
            var userName = User.FindFirstValue(ClaimTypes.Name); // Changed to ClaimTypes.Name
            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized(ApiResponse<object>.Fail("User not authenticated."));
            }

            var result = await _userProfileService.UpdateUserProfileAsync(userName, updateUserProfileDto); // Pass userName
            if (!result)
            {
                return BadRequest(ApiResponse<object>.Fail($"Failed to update user profile.{ErrorCode.ValidationError.Message}"));
            }

            return Ok(ApiResponse<string>.Ok("User profile updated successfully."));
        }
    }
}