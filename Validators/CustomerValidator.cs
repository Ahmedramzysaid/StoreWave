using FluentValidation;
using StoreWave.DTOs;

namespace StoreWave.Validators
{
    public class CustomerValidator : AbstractValidator<CustomerDto>
    {
        public CustomerValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(50).WithMessage("First name cannot exceed 50 characters");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(100).WithMessage("Email cannot exceed 100 characters");

            RuleFor(x => x.Phone)
                .Matches(@"^\+?[\d\s-]{10,}$")
                .When(x => !string.IsNullOrEmpty(x.Phone))
                .WithMessage("Invalid phone format");
                
            RuleFor(x => x.PostalCode)
                .Matches(@"^\d{5}(-\d{4})?$")
                .When(x => !string.IsNullOrEmpty(x.PostalCode) && x.Country == "USA")
                .WithMessage("Invalid US postal code format");
        }
    }
}
