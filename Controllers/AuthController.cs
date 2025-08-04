using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApplication1.Domain.Entities;
using WebApplication1.Application.DTOs.Users;
using Microsoft.AspNetCore.Authorization;
using WebApplication1.Application.Interfaces;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IApplicationUserService _applicationUserService;

        public AuthController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IApplicationUserService applicationUserService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _applicationUserService = applicationUserService;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = "User already exists!" });

            ApplicationUser user = new() { Email = model.Email, SecurityStamp = Guid.NewGuid().ToString(), UserName = model.Username };
            var result = await _userManager.CreateAsync(user, "Abc@123");
            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = "User creation failed! Please check user details and try again.", errors = result.Errors });
            }

            // Assign the default "User" role to the new user
            if (await _roleManager.RoleExistsAsync("User"))
            {
                await _userManager.AddToRoleAsync(user, "User");
            }

            return Ok(new { success = true, message = "User created successfully!" });
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo,
                    roles = userRoles // Return roles to the client
                });
            }
            return Unauthorized();
        }

        [HttpPut]
        [Route("update-user-role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUserRole([FromBody] UpdateUserRoleDto model)
        {
            // Ensure the new role is either "User" or "Staff"
            if (model.NewRole != "User" && model.NewRole != "Staff" && model.NewRole != "Admin")
            {
                return BadRequest(new { success = false, message = "Invalid role specified. Role must be 'User', 'Staff' or 'Admin'." });
            }

            try
            {
                var success = await _applicationUserService.UpdateUserRoleAsync(model);
                if (success)
                {
                    return Ok(new { success = true, message = $"User role and information updated successfully!" });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = "Failed to update user role or information. Check if you are trying to demote an admin." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = ex.Message });
            }
        }
    }
}