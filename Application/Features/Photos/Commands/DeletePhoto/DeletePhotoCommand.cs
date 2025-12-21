using MediatR;

namespace Agentic_Rentify.Application.Features.Photos.Commands.DeletePhoto;

/// <summary>
/// CQRS Command to delete a photo from Cloudinary.
/// Following the Delete command pattern, this command must be explicit about what is being deleted.
/// </summary>
public class DeletePhotoCommand : IRequest<bool>
{
    /// <summary>
    /// The Cloudinary public ID of the image to delete.
    /// </summary>
    public string PublicId { get; set; } = string.Empty;
}
