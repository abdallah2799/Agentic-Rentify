using Agentic_Rentify.Infragentic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Agentic_Rentify.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController(IChatAiService chatAiService) : ControllerBase
{
    /// <summary>
    /// Sends a message to the AI travel assistant and receives a response with automatic tool invocation.
    /// </summary>
    /// <param name="request">The chat request containing the user's message and conversation history.</param>
    /// <returns>AI-generated response with tool results (trip discovery, bookings, etc.)</returns>
    /// <remarks>
    /// The AI will automatically:
    /// - Search for trips/attractions when users ask about destinations
    /// - Create bookings and return Stripe payment URLs when users want to book
    /// - Provide helpful travel recommendations
    /// 
    /// The conversation array maintains context across messages.
    /// All function calls are logged for observability.
    /// </remarks>
    [HttpPost]
    public async Task<IActionResult> SendMessage([FromBody] ChatRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest(new { message = "Message is required" });
        }

        // Extract userId from JWT token
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var response = await chatAiService.GetResponseAsync(request.Message, userId, request.Conversation);
        
        // Build updated conversation history
        var updatedConversation = new List<ChatMessage>(request.Conversation ?? new List<ChatMessage>());
        updatedConversation.Add(new ChatMessage { Role = "user", Content = request.Message });
        updatedConversation.Add(new ChatMessage { Role = "assistant", Content = response });

        return Ok(new 
        { 
            message = response, 
            conversation = updatedConversation 
        });
    }
}

public class ChatRequest
{
    public string Message { get; set; } = string.Empty;
    public List<ChatMessage>? Conversation { get; set; }
}
