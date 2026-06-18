using EventBookingAPI.DTOs;
using FluentValidation;

namespace EventBookingAPI.Validation;

public class CreateEventDtoValid : AbstractValidator<CreateEventDto>
{
    public  CreateEventDtoValid()
    {
        RuleFor(e => e.Title)
            .NotEmpty()
            .WithMessage("Title is required")
            .MinimumLength(3)
            .WithMessage("Title must be between 3 and 100 characters")
            .MaximumLength(100)
            .WithMessage("Title cannot exceed 100 characters");

        RuleFor(e => e.Description)
            .NotEmpty()
            .WithMessage("Description is required")
            .MinimumLength(3)
            .WithMessage("Description must be between 3 and 1000 characters")
            .MaximumLength(1000)
            .WithMessage("Description cannot exceed 1000 characters");


        RuleFor(e => e.Location)
            .NotEmpty()
            .WithMessage("Location is required")
            .MinimumLength(3)
            .WithMessage("Location must be between 3 and 100 characters")
            .MaximumLength(100)
            .WithMessage("Location cannot exceed 100 characters");

        RuleFor(e => e.TotalSeats)
            .GreaterThan(0)
            .WithMessage("Total Seats must be greater than 0")
            .LessThan(1000)
            .WithMessage("Total Seats must be less than 1000");
        
        RuleFor(e => e.Price)
            .GreaterThan(0)
            .WithMessage("Price must be greater than 0")
            .LessThan(100000)
            .WithMessage("Price must be less than 100000");
    }
}