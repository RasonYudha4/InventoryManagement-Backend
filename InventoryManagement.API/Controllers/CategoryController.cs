using InventoryManagement.Application.Features.Categories.Commands.CreateCategory;
using InventoryManagement.Application.Features.Categories.Queries.GetAllCategories;
using InventoryManagement.Application.Features.Categories.Queries.GetCategoryTree;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController(IMediator mediator) : ControllerBase
{
    [Authorize(Roles = "Admin,Manager")]
    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryCommand command)
    {
        var id = await mediator.Send(command);
        return Ok(new { Message = "Category created successfully.", Id = id });
    }

    [Authorize(Roles = "Admin,Manager,WarehouseStaff,Auditor")]
    [HttpGet]
    public async Task<IActionResult> GetAllCategories()
    {
        var result = await mediator.Send(new GetAllCategoriesQuery());
        return Ok(result);
    }

    [Authorize(Roles = "Admin,Manager,WarehouseStaff,Auditor")]
    [HttpGet("tree")]
    public async Task<IActionResult> GetCategoryTree()
    {
        var result = await mediator.Send(new GetCategoryTreeQuery());
        return Ok(result);
    }
}