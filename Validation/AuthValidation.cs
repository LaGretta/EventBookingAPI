using EventBookingAPI.DTOs;
using FluentValidation;

namespace EventBookingAPI.Validation;

public class RegisterDtoValid : AbstractValidator<RegisterDto>
{
    public RegisterDtoValid()
    {
        RuleFor(x => x.Email)
            .EmailAddress()
            .NotEmpty()
            .WithMessage("Email is required");
        
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .MinimumLength(6)
            .WithMessage("Password must contain at least 6 characters")
            .MaximumLength(100)
            .WithMessage("Password must not exceed 100 characters");
        
        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage("Full name is required")
            .MinimumLength(3)
            .WithMessage("Full name must contain at least 3 characters")
            .MaximumLength(100)
            .WithMessage("Full name must not exceed 100 characters");
    }
}
public class LoginDtoValid : AbstractValidator<LoginDto>
{
    public  LoginDtoValid()
    {
        RuleFor(x => x.Email)
            .EmailAddress()
            .NotEmpty()
            .WithMessage("Email is required");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required");
    }
}
