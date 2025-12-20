using Agentic_Rentify.Application.Features.Bookings.Commands.CreateBooking;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace Agentic_Rentify.Infragentic.Plugins;

public class BookingPlugin(IServiceScopeFactory serviceScopeFactory)
{
    [KernelFunction("create_booking")]
    [Description("Creates a new booking for a trip, hotel, car, or attraction and initiates the Stripe payment checkout. Returns the Stripe session URL that the user must visit to complete payment. IMPORTANT: Always return the sessionUrl to the user so they can pay.")]
    public async Task<string> CreateBookingAsync(
        [Description("The authenticated user's ID")] string userId,
        [Description("The ID of the entity being booked (trip, hotel, car, or attraction)")] int entityId,
        [Description("The type of booking: 'Trip', 'Hotel', 'Car', or 'Attraction'")] string bookingType,
        [Description("Start date of the booking in ISO 8601 format (e.g., 2025-12-25T00:00:00Z)")] DateTime startDate,
        [Description("End date of the booking in ISO 8601 format. Optional for single-day bookings.")] DateTime? endDate,
        [Description("Total price in USD for the booking")] decimal totalPrice)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        
        var command = new CreateBookingCommand
        {
            UserId = userId,
            EntityId = entityId,
            BookingType = bookingType,
            StartDate = startDate,
            EndDate = endDate,
            TotalPrice = totalPrice
        };

        var sessionUrl = await mediator.Send(command);

        return $"Booking created successfully! Please complete your payment at: {sessionUrl}";
    }
}
