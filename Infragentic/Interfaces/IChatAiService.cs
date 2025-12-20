namespace Agentic_Rentify.Infragentic.Interfaces;

public interface IChatAiService
{
    Task<string> GetResponseAsync(string userMessage, List<ChatMessage>? conversationHistory = null);
}

public class ChatMessage
{
    public string Role { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}
