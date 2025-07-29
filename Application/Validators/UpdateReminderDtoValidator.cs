using FluentValidation;
using WebApplication1.Application.DTOs.Reminders;
using Microsoft.AspNetCore.Identity;
using WebApplication1.Domain.Entities;

namespace WebApplication1.Application.Validators
{
    public class UpdateReminderDtoValidator : AbstractValidator<UpdateReminderDto>
    {
        public UpdateReminderDtoValidator(UserManager<ApplicationUser> userManager)
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.UserId).SetValidator(new UserExistsValidator(userManager));
            RuleFor(x => x.Message).NotEmpty().MaximumLength(500);
            RuleFor(x => x.ReminderDateTime).NotEmpty();
        }
    }
}