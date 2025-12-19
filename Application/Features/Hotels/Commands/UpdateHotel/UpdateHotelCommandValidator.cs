using FluentValidation;

namespace Agentic_Rentify.Application.Features.Hotels.Commands.UpdateHotel;

public class UpdateHotelCommandValidator : AbstractValidator<UpdateHotelCommand>
{
    public UpdateHotelCommandValidator()
    {
        RuleFor(p => p.Id)
            .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.");
            
        RuleFor(p => p.Name)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .MaximumLength(200).WithMessage("{PropertyName} must not exceed 200 characters.");

        RuleFor(p => p.BasePrice)
            .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.");
    }
}
