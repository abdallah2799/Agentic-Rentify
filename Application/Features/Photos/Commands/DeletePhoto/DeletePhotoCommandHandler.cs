using Agentic_Rentify.Application.Interfaces;
using MediatR;

namespace Agentic_Rentify.Application.Features.Photos.Commands.DeletePhoto;

/// <summary>
/// Handler for DeletePhotoCommand.
/// Deletes a photo from Cloudinary using IPhotoService.
/// </summary>
public class DeletePhotoCommandHandler(IPhotoService photoService) : IRequestHandler<DeletePhotoCommand, bool>
{
    private readonly IPhotoService _photoService = photoService;

    /// <summary>
    /// Handles the deletion of a photo from Cloudinary.
    /// </summary>
    /// <param name="request">The command containing the PublicId of the photo to delete.</param>
    /// <param name="cancellationToken">Cancellation token for async operations.</param>
    /// <returns>True if deletion was successful; false otherwise.</returns>
    public async Task<bool> Handle(DeletePhotoCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.PublicId))
        {
            return false;
        }

        return await _photoService.DeletePhotoAsync(request.PublicId);
    }
}
