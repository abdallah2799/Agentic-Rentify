using Agentic_Rentify.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Agentic_Rentify.Api.Controllers;

/// <summary>
/// AI-powered conversational interface for travel discovery and booking
/// </summary>
/// <remarks>
/// This controller provides a natural language interface to the entire platform.
/// The AI assistant can search trips, attractions, create bookings, and provide recommendations.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "AI Operations")]
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
    /// **Response Interpretation:**
    /// 
    /// The response may include special UI hints in the message content:
    /// 
    /// - **UiHint Values:**
    ///   - `show_trips`: Display trip search results in a grid/list
    ///   - `redirect_payment`: Navigate user to Stripe checkout URL
    ///   - `booking_confirmed`: Show success screen with booking details
    ///   - `show_attractions`: Display attraction results
    /// 
    /// - **Payload Parsing:**
    ///   - When UiHint is present, parse the response for JSON blocks
    ///   - Extract structured data (trip objects, booking IDs, URLs)
    ///   - Use this data to populate UI components dynamically
    /// 
    /// The conversation array maintains context across messages.
    /// All function calls are logged for observability.
    /// 
    /// **Example Request:**
    /// ```json
    /// {
    ///   "message": "Find me romantic beach trips in Egypt",
    ///   "conversation": [
    ///     { "role": "user", "content": "Hello" },
    ///     { "role": "assistant", "content": "Hi! How can I help you today?" }
    ///   ]
    /// }
    /// ```
    /// 
    /// **Example Response:**
    /// ```json
    /// {
    ///   "message": "I found 3 romantic beach destinations...",
    ///   "conversation": [ ... updated history ... ]
    /// }
    /// ```
    /// </remarks>
    /// <response code="200">Returns AI response with updated conversation history</response>
    /// <response code="400">Message is required or validation failed</response>
    [HttpPost]
    [ProducesResponseType(typeof(ChatResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
    /// <summary>User's natural language message</summary>
    public string Message { get; set; } = string.Empty;
    
    /// <summary>Conversation history for context maintenance</summary>
    public List<ChatMessage>? Conversation { get; set; }
}

/// <summary>
/// Chat response with AI-generated message and updated conversation
/// </summary>
public class ChatResponse
{
    /// <summary>AI assistant's response (may contain UiHints and structured data)</summary>
    public string Message { get; set; } = string.Empty;
    
    /// <summary>Updated conversation history including this exchange</summary>
    public List<ChatMessage> Conversation { get; set; } = new();
}
