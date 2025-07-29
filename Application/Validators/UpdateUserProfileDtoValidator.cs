using FluentValidation;
using WebApplication1.Application.DTOs.Users;

namespace WebApplication1.Application.Validators
{
    public class UpdateUserProfileDtoValidator : AbstractValidator<UpdateUserProfileDto>
    {
        public UpdateUserProfileDtoValidator()
        {
            RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrEmpty(x.Email)).WithMessage("Invalid email format.");
            RuleFor(x => x.PhoneNumber).Matches(@"^\+?[0-9]{10,15}$").When(x => !string.IsNullOrEmpty(x.PhoneNumber)).WithMessage("Invalid phone number format.");
        }
    }
}