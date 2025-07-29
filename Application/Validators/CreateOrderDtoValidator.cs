using FluentValidation;
using WebApplication1.Application.DTOs.Orders;

namespace WebApplication1.Application.Validators
{
    public class CreateOrderDtoValidator : AbstractValidator<CreateOrderDto>
    {
        public CreateOrderDtoValidator()
        {
            RuleFor(x => x.OrderItems).NotEmpty().WithMessage("Order must contain at least one item.");
            RuleForEach(x => x.OrderItems).SetValidator(new CreateOrderItemDtoValidator());
        }
    }
}