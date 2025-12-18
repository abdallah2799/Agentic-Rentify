public class AttractionReviewSummary
{
    public double OverallRating { get; set; }
    public int TotalReviews { get; set; }
    public RatingCriteria Criteria { get; set; } = new();
}