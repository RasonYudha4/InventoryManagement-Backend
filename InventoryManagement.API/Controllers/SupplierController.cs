using InventoryManagement.API.Models.Requests;
using InventoryManagement.Application.Features.Suppliers.Queries.GetAllSuppliers;
using InventoryManagement.Application.Features.Suppliers.Queries.GetSupplierById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SupplierController(IMediator mediator) : ControllerBase
{
    [Authorize(Roles = "Admin,Manager")]
    [HttpPost]
    public async Task<IActionResult> CreateSupplier([FromBody] CreateSupplierRequest request)
    {
        var command = new CreateSupplierCommand(
            request.Name,
            request.ContactName,
            request.Email,
            request.Phone,
            request.Address,
            request.TaxId
        );

        var supplierId = await mediator.Send(command);
        return Ok(new { Message = "Supplier created successfully", Id = supplierId });
    }

    [Authorize(Roles = "Admin,Manager,Auditor")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetSupplierById(Guid id)
    {
        var query = new GetSupplierByIdQuery(id);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [Authorize(Roles = "Admin,Manager,Auditor")]
    [HttpGet]
    public async Task<IActionResult> GetAllSuppliers()
    {
        var result = await mediator.Send(new GetAllSuppliersQuery());
        return Ok(result);
    }
}