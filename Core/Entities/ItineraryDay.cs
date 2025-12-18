// Core -> Entities -> Helpers (أو في نفس ملف الـ Entity)
public class ItineraryDay
{
    public int Day { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<Activity> Activities { get; set; } = [];
}

