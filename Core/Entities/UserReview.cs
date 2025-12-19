namespace Agentic_Rentify.Core.Entities;

public class UserReview
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserImage { get; set; } = string.Empty;
    public double Rating { get; set; }
    public string Date { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
}
