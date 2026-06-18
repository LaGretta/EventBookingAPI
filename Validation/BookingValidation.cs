using EventBookingAPI.DTOs;
using FluentValidation;

namespace EventBookingAPI.Validation;

public class CreateBookingDtoValid : AbstractValidator<CreateBookingDto>
{
    public CreateBookingDtoValid()
    {
        RuleFor(b => b.EventId)
            .GreaterThan(0)
            .WithMessage("Event id is required");

        RuleFor(b => b.SeatsCount)
            .GreaterThan(0)
            .WithMessage("Seats count must be greater than zero")
            .LessThanOrEqualTo(10000);
      
        
        RuleFor(b => b.TotalPrice)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Total price must be greater than zero")
            .LessThanOrEqualTo(1000000000);
    }
}
