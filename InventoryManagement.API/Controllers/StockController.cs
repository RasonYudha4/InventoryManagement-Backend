using System.Security.Claims;
using InventoryManagement.API.Models;
using InventoryManagement.Application.Features.Stock.Commands.ReceiveStock;
using InventoryManagement.Application.Features.Stock.Queries.GetStockLevelsByLocation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StockController(IMediator mediator) : ControllerBase
{
    [HttpPost("receive")]
    public async Task<IActionResult> ReceiveStock([FromBody] ReceiveStockRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User identity not found in token.");

        var command = new ReceiveStockCommand(
            request.ProductId,
            request.LocationId,
            request.Quantity,
            request.ReferenceNumber,
            request.Notes,
            userId
        );

        var transactionId = await mediator.Send(command);

        return Ok(new { Message = "Stock received successfully", TransactionId = transactionId});
    }

    [HttpGet("location/{locationId}")]
    public async Task<IActionResult> GetStockByLocation(Guid locationId)
    {
        var query = new GetStockLevelsByLocationQuery(locationId);
        
        var result = await mediator.Send(query);

        return Ok(result);
    }
}