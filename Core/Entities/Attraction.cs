// Core -> Entities -> Attraction.cs
public class Attraction : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public double Rating { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Overview { get; set; } = string.Empty;
    public decimal Price { get; set; } // حولناه لـ Decimal
    public string Currency { get; set; } = "$";

    // الإحداثيات عشان الـ Qdrant مستقبلاً
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    // Navigation Properties
    public List<AttractionImage> Images { get; set; } = [];
    public List<Category> Categories { get; set; } = [];

    // الحقول المعقدة اللي هتبقى JSON في الداتابيز
    public AttractionReviewSummary ReviewSummary { get; set; } = new();
    public List<string> Amenities { get; set; } = [];
    public List<string> Highlights { get; set; } = [];
}