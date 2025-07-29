using FluentValidation;
using WebApplication1.Application.DTOs.Categories;

namespace WebApplication1.Application.Validators
{
    public class CreateCategoryDtoValidator : AbstractValidator<CreateCategoryDto>
    {
        public CreateCategoryDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Category name is required.");
            RuleFor(x => x.Name).MaximumLength(100).WithMessage("Category name cannot exceed 100 characters.");
            RuleFor(x => x.Description).MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
        }
    }
}