using FluentValidation;
using StoreWave.DTOs;

namespace StoreWave.Validators
{
    public class OrderValidator : AbstractValidator<OrderDto>
    {
        public OrderValidator()
        {
            RuleFor(x => x.ShippingAddress)
                .NotEmpty().WithMessage("Shipping address is required")
                .MaximumLength(500);

            RuleFor(x => x.ShippingCity)
                .NotEmpty().WithMessage("City is required")
                .MaximumLength(100);

            RuleFor(x => x.ShippingCountry)
                .NotEmpty().WithMessage("Country is required")
                .MaximumLength(100);

            RuleFor(x => x.CustomerId)
                .GreaterThan(0).WithMessage("Customer is required");
                
            RuleFor(x => x.OrderItems)
                .Must(items => items != null && items.Count > 0)
                .When(x => x.Id == 0) // Only validate items on creation
                .WithMessage("Order must contain at least one item");
        }
    }
}
