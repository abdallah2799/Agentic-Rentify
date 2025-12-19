using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

public class GlobalExceptionHandler : IMiddleware
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        HttpStatusCode statusCode;
        string message = exception.Message;

        switch (exception)
        {
            case BadRequestException:
                statusCode = HttpStatusCode.BadRequest;
                break;
            case UnauthorizedException:
                statusCode = HttpStatusCode.Unauthorized;
                break;
            case NotFoundException:
                statusCode = HttpStatusCode.NotFound;
                break;
            case FluentValidation.ValidationException validationEx:
                statusCode = HttpStatusCode.BadRequest;
                var errors = validationEx.Errors.Select(e => e.ErrorMessage).ToList();
                message = JsonSerializer.Serialize(errors);
                break;
            default:
                statusCode = HttpStatusCode.InternalServerError;
                message = "An internal server error occurred.";
                break;
        }

        context.Response.StatusCode = (int)statusCode;

        var response = new ProblemDetails
        {
            Status = (int)statusCode,
            Detail = message,
            Type = exception.GetType().Name
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
