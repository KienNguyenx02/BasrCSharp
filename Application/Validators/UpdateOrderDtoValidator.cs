using FluentValidation;
using WebApplication1.Application.DTOs.Orders;

namespace WebApplication1.Application.Validators
{
    public class UpdateOrderDtoValidator : AbstractValidator<UpdateOrderDto>
    {
        public UpdateOrderDtoValidator()
        {
            RuleFor(x => x.Status).NotEmpty().WithMessage("Status is required.");
            // You might want to add more specific validation for allowed status values
        }
    }
}