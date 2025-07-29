using FluentValidation;
using WebApplication1.Application.DTOs.Orders;

namespace WebApplication1.Application.Validators
{
    public class CreateOrderItemDtoValidator : AbstractValidator<CreateOrderItemDto>
    {
        // public CreateOrderItemDtoValidator()
        // {
        //     RuleFor(x => x.ProductId).GreaterThan(0).WithMessage("Product ID must be greater than 0.");
        //     RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than 0.");
        // }
    }
}