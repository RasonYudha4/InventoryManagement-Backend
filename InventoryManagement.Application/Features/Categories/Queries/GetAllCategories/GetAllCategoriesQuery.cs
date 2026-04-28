using MediatR;

namespace InventoryManagement.Application.Features.Categories.Queries.GetAllCategories;

public record CategorySummaryDto(
    Guid Id,
    string Name,
    string? Description,
    Guid? ParentCategoryId,
    string? ParentCategoryName
);

public record GetAllCategoriesQuery : IRequest<List<CategorySummaryDto>>;