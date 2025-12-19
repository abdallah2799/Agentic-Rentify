using System.Net;
using System.Text.Json;
using FluentValidation;
using Stripe;

namespace Agentic_Rentify.Api.Middleware;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        _logger.LogError(ex, "An unexpected error occurred.");

        context.Response.ContentType = "application/json";

        if (ex is ValidationException validationException)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            var validationErrors = validationException.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            var response = new
            {
                statusCode = context.Response.StatusCode,
                message = "Validation failed",
                errors = validationErrors
            };
            
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
        else if (ex is StripeException stripeException)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            var response = new
            {
                statusCode = context.Response.StatusCode,
                message = stripeException.Message,
                stripeError = stripeException.StripeError?.Message
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
        else
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            var response = new
            {
                statusCode = context.Response.StatusCode,
                message = ex.Message,
                stackTrace = _env.IsDevelopment() ? ex.StackTrace : null
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
