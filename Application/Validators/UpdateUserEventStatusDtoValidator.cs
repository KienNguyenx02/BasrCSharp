using FluentValidation;
using WebApplication1.Application.DTOs.UserEventStatus;
using Microsoft.AspNetCore.Identity;
using WebApplication1.Domain.Entities;

namespace WebApplication1.Application.Validators
{
    public class UpdateUserEventStatusDtoValidator : AbstractValidator<UpdateUserEventStatusDto>
    {
        public UpdateUserEventStatusDtoValidator(UserManager<ApplicationUser> userManager)
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.EventId).NotEmpty();
            RuleFor(x => x.UserId).SetValidator(new UserExistsValidator(userManager));
            RuleFor(x => x.Status).IsInEnum();
        }
    }
}