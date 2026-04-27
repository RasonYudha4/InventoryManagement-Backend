using Microsoft.AspNetCore.Mvc;
using InventoryManagement.Application.Interfaces;

namespace InventoryManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AiController : ControllerBase
{
    private readonly IAiCopilotService _aiCopilotService;

    public AiController(IAiCopilotService aiCopilotService)
    {
        _aiCopilotService = aiCopilotService;
    }

    [HttpPost("ask")]
    public async Task<IActionResult> AskWarehouseQuestion([FromBody] AiQuestionRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Question))
        {
            return BadRequest("You must ask a question.");
        }

        try
        {
            // We pass a dummy "System" userId for now. Later, you can grab the real user's ID from the JWT token!
            string answer = await _aiCopilotService.AskWarehouseQuestionAsync(request.Question, "System");
            
            // We wrap the response in an object so the frontend can easily parse the JSON
            return Ok(new { Answer = answer });
        }
        catch (Exception ex)
        {
            // Print the massive, detailed error to your VS Code terminal in red
            Console.WriteLine(ex.ToString()); 
            
            // Send the Inner Exception to the Swagger UI
            string innerError = ex.InnerException != null ? ex.InnerException.Message : "No inner exception";
            return StatusCode(500, new { Error = ex.Message, InnerDetails = innerError });
        }
    }
}

// A simple DTO (Data Transfer Object) to catch the incoming JSON body
public class AiQuestionRequest
{
    public string Question { get; set; } = string.Empty;
}