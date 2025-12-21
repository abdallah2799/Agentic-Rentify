using FluentValidation;

namespace Agentic_Rentify.Application.Features.Photos.Commands.DeletePhoto;

/// <summary>
/// Validator for DeletePhotoCommand.
/// Ensures that the PublicId is valid before attempting deletion.
/// </summary>
public class DeletePhotoCommandValidator : AbstractValidator<DeletePhotoCommand>
{
    public DeletePhotoCommandValidator()
    {
        RuleFor(cmd => cmd.PublicId)
            .NotEmpty()
            .WithMessage("PublicId is required for photo deletion.")
            .MaximumLength(500)
            .WithMessage("PublicId must not exceed 500 characters.");
    }
}
