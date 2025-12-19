using FluentValidation;

namespace Agentic_Rentify.Application.Features.Attractions.Commands.CreateAttraction;

public class CreateAttractionCommandValidator : AbstractValidator<CreateAttractionCommand>
{
    public CreateAttractionCommandValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .NotNull()
            .MaximumLength(100).WithMessage("{PropertyName} must not exceed 100 characters.");

        RuleFor(p => p.City)
            .NotEmpty().WithMessage("{PropertyName} is required.");

        RuleFor(p => p.Price)
            .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} cannot be negative.");

        RuleFor(p => p.Latitude)
            .InclusiveBetween(-90, 90).WithMessage("{PropertyName} must be between -90 and 90.");

        RuleFor(p => p.Longitude)
            .InclusiveBetween(-180, 180).WithMessage("{PropertyName} must be between -180 and 180.");
    }
}
