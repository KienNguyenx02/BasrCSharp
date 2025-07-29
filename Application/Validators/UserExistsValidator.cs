using FluentValidation;
using Microsoft.AspNetCore.Identity;
using WebApplication1.Domain.Entities;
using System.Threading.Tasks;

namespace WebApplication1.Application.Validators
{
    public class UserExistsValidator : AbstractValidator<Guid>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserExistsValidator(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;

            RuleFor(userId => userId)
                .MustAsync(UserMustExist)
                .WithMessage("User with the specified ID does not exist.");
        }

        private async Task<bool> UserMustExist(Guid userId, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            return user != null;
        }
    }
}