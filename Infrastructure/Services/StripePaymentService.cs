using Agentic_Rentify.Application.Interfaces;
using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace Agentic_Rentify.Infrastructure.Services;

public class StripePaymentService : IPaymentService
{
    private readonly StripeSettings _settings;
    private readonly string _clientAppUrl;

    public StripePaymentService(IOptions<StripeSettings> stripeOptions, IConfiguration configuration)
    {
        _settings = stripeOptions.Value;
        _clientAppUrl = configuration["ClientAppUrl"] ?? configuration["AppBaseUrl"] ?? "http://localhost:3000";
        StripeConfiguration.ApiKey = _settings.SecretKey;
    }

    public async Task<string> CreateCheckoutSessionAsync(Booking booking)
    {
        var sessionOptions = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            Mode = "payment",
            SuccessUrl = $"{_clientAppUrl}/payment-success?sessionId={{CHECKOUT_SESSION_ID}}",
            CancelUrl = $"{_clientAppUrl}/payment-cancelled",
            Metadata = new Dictionary<string, string>
            {
                { "bookingId", booking.Id.ToString() },
                { "userId", booking.UserId },
                { "bookingType", booking.BookingType }
            },
            LineItems = new List<SessionLineItemOptions>
            {
                new()
                {
                    Quantity = 1,
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        UnitAmountDecimal = booking.TotalPrice * 100,
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = $"{booking.BookingType} Booking #{booking.Id}",
                            Metadata = new Dictionary<string, string>
                            {
                                { "entityId", booking.EntityId.ToString() }
                            }
                        }
                    }
                }
            }
        };

        var sessionService = new SessionService();
        var session = await sessionService.CreateAsync(sessionOptions);

        booking.StripeSessionId = session.Id;
        return session.Url ?? string.Empty;
    }
}
