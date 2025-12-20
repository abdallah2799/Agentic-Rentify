using Agentic_Rentify.Application.Features.Bookings.Commands.ConfirmBooking;
using Agentic_Rentify.Application.Features.Bookings.Commands.UpdateBooking;
using Agentic_Rentify.Application.Features.Bookings.Commands.DeleteBooking;
using Agentic_Rentify.Application.Features.Bookings.Commands.CancelBooking;
using Agentic_Rentify.Application.Features.Bookings.Queries.GetActiveBookings;
using Agentic_Rentify.Application.Features.Bookings.Queries.GetBookingById;
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
    /// Gets all active bookings (excludes cancelled/failed/soft-deleted).
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetActive()
    {
        var items = await mediator.Send(new GetActiveBookingsQuery());
        return Ok(items);
    }

    /// <summary>
    /// Get a booking by ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await mediator.Send(new GetBookingByIdQuery(id));
        return Ok(item);
    }

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
    /// Update booking dates/price.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateBookingCommand command)
    {
        if (id != command.Id) return BadRequest(new { message = "ID mismatch" });
        var resultId = await mediator.Send(command);
        return Ok(new { id = resultId });
    }

    /// <summary>
    /// Cancel a booking. If already paid, marks as Cancelled (refund not implemented).
    /// </summary>
    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        await mediator.Send(new CancelBookingCommand(id));
        return Ok(new { id, status = "cancelled" });
    }

    /// <summary>
    /// Soft-delete a booking (marks as deleted and cancelled).
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await mediator.Send(new DeleteBookingCommand(id));
        return Ok(new { id, deleted = true });
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
