using Agentic_Rentify.Core.Common;
using Agentic_Rentify.Core.Enums;

namespace Agentic_Rentify.Core.Entities;

public class Booking : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public int EntityId { get; set; }
    public string BookingType { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal TotalPrice { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    public string StripeSessionId { get; set; } = string.Empty;
    public bool IsPaid { get; set; }
}
