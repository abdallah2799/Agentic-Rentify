using MediatR;

public class ValidateTokenQuery : IRequest<bool>
{
    public string Token { get; set; } = string.Empty;
}

public class ValidateTokenQueryHandler : IRequestHandler<ValidateTokenQuery, bool>
{
    private readonly IIdentityService _identityService;

    public ValidateTokenQueryHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<bool> Handle(ValidateTokenQuery request, CancellationToken cancellationToken)
    {
        return await _identityService.ValidateTokenAsync(request.Token);
    }
}
