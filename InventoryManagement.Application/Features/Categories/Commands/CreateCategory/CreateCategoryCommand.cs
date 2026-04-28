using MediatR;

namespace InventoryManagement.Application.Features.Categories.Commands.CreateCategory;

public record CreateCategoryCommand(
    string Name,
    string? Description,
    Guid? ParentCategoryId
) : IRequest<Guid>;