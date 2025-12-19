using MediatR;

public record ResendOtpCommand(string Email) : IRequest;

public class ResendOtpCommandHandler : IRequestHandler<ResendOtpCommand>
{
    private readonly IIdentityService _identityService;

    public ResendOtpCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task Handle(ResendOtpCommand request, CancellationToken cancellationToken)
    {
        await _identityService.ResendOtpAsync(request.Email);
    }
}
