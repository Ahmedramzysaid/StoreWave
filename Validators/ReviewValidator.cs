using FluentValidation;
using StoreWave.DTOs;

namespace StoreWave.Validators
{
    public class ReviewValidator : AbstractValidator<ReviewDto>
    {
        public ReviewValidator()
        {
            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5 stars");

            RuleFor(x => x.Title)
                .MaximumLength(100).WithMessage("Title cannot exceed 100 characters");

            RuleFor(x => x.Comment)
                .MaximumLength(2000).WithMessage("Comment cannot exceed 2000 characters");

            RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("Product is required");

            RuleFor(x => x.CustomerId)
                .GreaterThan(0).WithMessage("Customer is required");
        }
    }
}
