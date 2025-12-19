using Agentic_Rentify.Application.Features.Bookings.Commands.ConfirmBooking;
using Agentic_Rentify.Application.Features.Bookings.Commands.CreateBooking;
using Agentic_Rentify.Infrastructure.Settings;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace Agentic_Rentify.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController(IMediator mediator, IOptions<StripeSettings> stripeOptions, ILogger<BookingsController> logger) : ControllerBase
{
    private readonly StripeSettings _stripeSettings = stripeOptions.Value;

    /// <summary>
    /// Creates a new booking and initiates a Stripe checkout session.
    /// </summary>
    /// <param name="command">The booking command containing user, entity, type, dates, and total price.</param>
    /// <returns>A JSON response with the Stripe checkout session URL.</returns>
    /// <remarks>
    /// The booking is initially created with status "Pending" and IsPaid=false.
    /// The returned sessionUrl should be redirected to on the client to complete payment.
    /// </remarks>
    /// <response code="200">Returns the checkout session URL for Stripe payment.</response>
    /// <response code="400">Validation failed or Stripe API error.</response>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBookingCommand command)
    {
        var sessionUrl = await mediator.Send(command);
        return Ok(new { sessionUrl });
    }

    /// <summary>
    /// Webhook endpoint for Stripe checkout.session.completed events.
    /// </summary>
    /// <remarks>
    /// This endpoint:
    /// 1. Verifies the Stripe webhook signature using WebhookSecret.
    /// 2. Extracts the session ID from the Stripe event.
    /// 3. Updates the booking status to "Confirmed" and sets IsPaid=true.
    /// 
    /// Configure this URL in Stripe Dashboard: POST /api/bookings/webhook
    /// </remarks>
    /// <response code="200">Webhook received and processed successfully.</response>
    /// <response code="400">Invalid webhook signature or malformed payload.</response>
    [HttpPost("webhook")]
    public async Task<IActionResult> HandleWebhook()
    {
        var payload = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        var signatureHeader = Request.Headers["Stripe-Signature"]; // Stripe sends lower case header name

        Event stripeEvent;
        try
        {
            stripeEvent = EventUtility.ConstructEvent(payload, signatureHeader, _stripeSettings.WebhookSecret);
        }
        catch (StripeException ex)
        {
            logger.LogWarning(ex, "Stripe webhook signature validation failed.");
            return BadRequest(new { message = "Invalid webhook signature" });
        }

        if (stripeEvent.Type == "checkout.session.completed" && stripeEvent.Data.Object is Session session)
        {
            await mediator.Send(new ConfirmBookingCommand(session.Id));
        }

        return Ok(new { received = true });
    }
}
