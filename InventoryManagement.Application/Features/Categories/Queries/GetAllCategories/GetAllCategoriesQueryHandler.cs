using InventoryManagement.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Application.Features.Categories.Queries.GetAllCategories;

public class GetAllCategoriesQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetAllCategoriesQuery, List<CategorySummaryDto>>
{
    public async Task<List<CategorySummaryDto>> Handle(
        GetAllCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        return await context.Categories
            .Include(c => c.ParentCategory)
            .OrderBy(c => c.ParentCategoryId == null ? 0 : 1)
                .ThenBy(c => c.Name)
            .Select(c => new CategorySummaryDto(
                c.Id,
                c.Name,
                c.Description,
                c.ParentCategoryId,
                c.ParentCategory != null ? c.ParentCategory.Name : null))
            .ToListAsync(cancellationToken);
    }
}