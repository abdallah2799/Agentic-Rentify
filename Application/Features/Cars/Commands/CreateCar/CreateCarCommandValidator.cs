using FluentValidation;

namespace Agentic_Rentify.Application.Features.Cars.Commands.CreateCar;

public class CreateCarCommandValidator : AbstractValidator<CreateCarCommand>
{
    public CreateCarCommandValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .MaximumLength(200).WithMessage("{PropertyName} must not exceed 200 characters.");

        RuleFor(p => p.Price)
            .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.");
            
        RuleFor(p => p.Seats)
            .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.");
    }
}
