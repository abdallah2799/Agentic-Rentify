using Agentic_Rentify.Core.Entities;

namespace Agentic_Rentify.Application.Interfaces;

public interface IPaymentService
{
    Task<string> CreateCheckoutSessionAsync(Booking booking);
}
