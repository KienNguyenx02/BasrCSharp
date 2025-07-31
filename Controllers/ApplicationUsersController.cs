using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.Application.DTOs.Users;
using WebApplication1.Application.Interfaces;
using WebApplication1.Shared.Results;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicationUsersController : ControllerBase
    {
        private readonly IApplicationUserService _applicationUserService;

        public ApplicationUsersController(IApplicationUserService applicationUserService)
        {
            _applicationUserService = applicationUserService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<object>>> GetAllUsers([FromQuery] FilterParams filterParams)
        {
            var users = await _applicationUserService.GetAllUsersAsync(filterParams);
            return Ok(ApiResponse<object>.Ok(users));
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<ApiResponse<ApplicationUserDto>>> GetUserById(string userId)
        {
            var user = await _applicationUserService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound(ApiResponse<ApplicationUserDto>.Fail("User not found."));
            }
            return Ok(ApiResponse<ApplicationUserDto>.Ok(user));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<ApplicationUserDto>>> CreateUser([FromBody] CreateApplicationUserDto createUserDto)
        {
            try
            {
                var user = await _applicationUserService.CreateUserAsync(createUserDto);
                return CreatedAtAction(nameof(GetUserById), new { userId = user.Id }, ApiResponse<ApplicationUserDto>.Ok(user));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<ApplicationUserDto>.Fail(ex.Message));
            }
        }

        [HttpPut("{userId}")]
        public async Task<ActionResult<ApiResponse<string>>> UpdateUser(string userId, [FromBody] UpdateApplicationUserDto updateUserDto)
        {
            if (userId != updateUserDto.Id)
            {
                return BadRequest(ApiResponse<string>.Fail("User ID mismatch."));
            }
            try
            {
                var result = await _applicationUserService.UpdateUserAsync(updateUserDto);
                if (!result)
                {
                    return NotFound(ApiResponse<string>.Fail("User not found."));
                }
                return Ok(ApiResponse<string>.Ok("User updated successfully."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.Fail(ex.Message));
            }
        }

        [HttpDelete("{userId}")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteUser(string userId)
        {
            var result = await _applicationUserService.DeleteUserAsync(userId);
            if (!result)
            {
                return NotFound(ApiResponse<string>.Fail("User not found."));
            }
            return Ok(ApiResponse<string>.Ok("User deleted successfully."));
        }
    }
}