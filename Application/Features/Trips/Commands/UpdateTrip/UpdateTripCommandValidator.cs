using FluentValidation;

namespace Agentic_Rentify.Application.Features.Trips.Commands.UpdateTrip;

public class UpdateTripCommandValidator : AbstractValidator<UpdateTripCommand>
{
    public UpdateTripCommandValidator()
    {
        RuleFor(p => p.Id)
            .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.");
            
        RuleFor(p => p.Title)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .MaximumLength(250).WithMessage("{PropertyName} must not exceed 250 characters.");

        RuleFor(p => p.Price)
            .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.");
            
        RuleFor(p => p.MaxPeople)
             .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.");
    }
}
