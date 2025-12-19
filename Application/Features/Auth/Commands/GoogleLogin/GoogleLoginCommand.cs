using MediatR;

public class GoogleLoginCommand : IRequest<AuthResponseDto>
{
    public string IdToken { get; set; } = string.Empty;
}

public class GoogleLoginCommandHandler : IRequestHandler<GoogleLoginCommand, AuthResponseDto>
{
    private readonly IIdentityService _identityService;

    public GoogleLoginCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<AuthResponseDto> Handle(GoogleLoginCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.LoginWithGoogleAsync(request.IdToken);
    }
}
