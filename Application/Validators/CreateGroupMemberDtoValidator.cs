using FluentValidation;
using WebApplication1.Application.DTOs.GroupMembers;
using Microsoft.AspNetCore.Identity;
using WebApplication1.Domain.Entities;

namespace WebApplication1.Application.Validators
{
    public class CreateGroupMemberDtoValidator : AbstractValidator<CreateGroupMemberDto>
    {
        public CreateGroupMemberDtoValidator(UserManager<ApplicationUser> userManager)
        {
            RuleFor(x => x.GroupId).NotEmpty();
            RuleFor(x => x.UserId).SetValidator(new UserExistsValidator(userManager));
            RuleFor(x => x.JoinedDate).NotEmpty();
        }
    }
}