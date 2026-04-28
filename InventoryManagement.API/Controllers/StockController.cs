using System.Security.Claims;
using InventoryManagement.API.Models;
using InventoryManagement.Application.Features.Stock.Commands.DispatchStock;
using InventoryManagement.Application.Features.Stock.Commands.ReceiveStock;
using InventoryManagement.Application.Features.Stock.Queries;
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
    [Authorize(Roles = "Admin,Manager,WarehouseStaff")]
    [HttpPost("receive")]
    public async Task<IActionResult> ReceiveStock([FromBody] ReceiveStockRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User identity not found in token.");

        var command = new ReceiveStockCommand(
            request.ProductId,
            request.LocationId,
            request.PurchaseOrderId,
            request.Quantity,
            request.Notes
        );

        var transactionId = await mediator.Send(command);
        return Ok(new { Message = "Stock received successfully", TransactionId = transactionId });
    }

    [Authorize(Roles = "Admin,Manager,WarehouseStaff,Auditor")]
    [HttpGet("location/{locationId}")]
    public async Task<IActionResult> GetStockByLocation(Guid locationId)
    {
        var query = new GetStockLevelsByLocationQuery(locationId);
        var result = await mediator.Send(query);

        return Ok(result);
    }

    [Authorize(Roles = "Admin,Manager,WarehouseStaff")]
    [HttpPost("dispatch")]
    public async Task<IActionResult> DispatchStock([FromBody] DispatchStockRequest request)
    {
        var command = new DispatchStockCommand(
            request.ProductId,
            request.LocationId,
            request.Quantity,
            request.SalesOrderNumber,
            request.Notes
        );

        var transactionId = await mediator.Send(command);
        return Ok(new { Message = "Stock dispatched successfully.", transactionId });
    }

    [Authorize(Roles = "Admin,Manager,WarehouseStaff,Auditor")]
    [HttpGet("low")]
    public async Task<IActionResult> GetLowStockItems()
    {
        var query = new GetLowStockQuery();
        var result = await mediator.Send(query);

        return Ok(result);
    }
}