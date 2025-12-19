namespace Agentic_Rentify.Core.Entities;

public class ItineraryDay
{
    public int Day { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<Activity> Activities { get; set; } = [];
}
