using FluentValidation;
using StoreWave.DTOs;

namespace StoreWave.Validators
{
    public class ProductValidator : AbstractValidator<ProductDto>
    {
        public ProductValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Product name is required")
                .MaximumLength(200).WithMessage("Name cannot exceed 200 characters");

            RuleFor(x => x.Description)
                .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than zero");

            RuleFor(x => x.DiscountPrice)
                .LessThan(x => x.Price)
                .When(x => x.DiscountPrice.HasValue)
                .WithMessage("Discount price must be less than regular price");

            RuleFor(x => x.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Stock cannot be negative");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("Please select a category");
        }
    }
}
