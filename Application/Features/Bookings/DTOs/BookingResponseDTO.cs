using Agentic_Rentify.Core.Enums;

namespace Agentic_Rentify.Application.Features.Bookings.DTOs;

public class BookingResponseDTO
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int EntityId { get; set; }
    public string BookingType { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal TotalPrice { get; set; }
    public BookingStatus Status { get; set; }
    public bool IsPaid { get; set; }
}
