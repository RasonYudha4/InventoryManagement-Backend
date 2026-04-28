using MediatR;

namespace InventoryManagement.Application.Features.Categories.Queries.GetCategoryTree;

public record CategoryTreeNodeDto(
    Guid Id,
    string Name,
    string? Description,
    List<CategoryTreeNodeDto> SubCategories
);

public record GetCategoryTreeQuery : IRequest<List<CategoryTreeNodeDto>>;