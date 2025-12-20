namespace Agentic_Rentify.Infragentic.Settings;

public class AiSettings
{
    public string OpenAIKey { get; set; } = string.Empty;
    public string ChatModel { get; set; } = string.Empty;
    public string PlannerModel { get; set; } = string.Empty;
    public string EmbeddingModel { get; set; } = string.Empty;
    public string OpenAIEndpoint { get; set; } = string.Empty;
    public string QdrantEndpoint { get; set; } = string.Empty;
    public string QdrantHost { get; set; } = string.Empty;
    public int QdrantPort { get; set; }
}
