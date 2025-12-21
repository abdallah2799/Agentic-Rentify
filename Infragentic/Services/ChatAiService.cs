using Agentic_Rentify.Application.Interfaces;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace Agentic_Rentify.Infragentic.Services;

public class ChatAiService(Kernel kernel) : IChatAiService
{
    public async Task<string> GetResponseAsync(string userMessage, string? userId = null, List<ChatMessage>? conversationHistory = null)
    {
        var executionSettings = new OpenAIPromptExecutionSettings
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
            Temperature = 0.7,
            MaxTokens = 2000
        };

        var chatHistory = new ChatHistory();
        
        chatHistory.AddSystemMessage(@"You are an intelligent travel assistant for Agentic Rentify. 
Your role is to help users discover trips, attractions, hotels, and cars, and assist them with bookings.

Key behaviors:
- When users ask about trips or attractions, use the available tools to search and present options.
- When users want to book, use the create_booking tool and ALWAYS return the payment URL to the user.
- Be friendly, concise, and helpful.
- Format responses in a clear, conversational manner.");

        // Add conversation history if provided
        if (conversationHistory != null && conversationHistory.Any())
        {
            foreach (var msg in conversationHistory)
            {
                if (msg.Role.Equals("user", StringComparison.OrdinalIgnoreCase))
                {
                    chatHistory.AddUserMessage(msg.Content);
                }
                else if (msg.Role.Equals("assistant", StringComparison.OrdinalIgnoreCase))
                {
                    chatHistory.AddAssistantMessage(msg.Content);
                }
            }
        }

        // Add current user message
        chatHistory.AddUserMessage(userMessage);

        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
        
        // Create invoke options with userId for filter access
        var invokeOptions = new KernelArguments(executionSettings);
        if (!string.IsNullOrWhiteSpace(userId))
        {
            invokeOptions["UserId"] = userId;
        }
        
        var result = await chatCompletionService.GetChatMessageContentAsync(
            chatHistory,
            executionSettings,
            kernel
        );

        return result.Content ?? string.Empty;
    }
}
