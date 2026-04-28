using InventoryManagement.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Application.Features.Categories.Queries.GetCategoryTree;

public class GetCategoryTreeQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetCategoryTreeQuery, List<CategoryTreeNodeDto>>
{
    public async Task<List<CategoryTreeNodeDto>> Handle(
        GetCategoryTreeQuery request,
        CancellationToken cancellationToken)
    {
        // Load all categories in a single query then build the tree in memory
        // to avoid N+1 recursive database calls
        var all = await context.Categories
            .OrderBy(c => c.Name)
            .Select(c => new { c.Id, c.Name, c.Description, c.ParentCategoryId })
            .ToListAsync(cancellationToken);

        var lookup = all.ToLookup(c => c.ParentCategoryId);

        List<CategoryTreeNodeDto> BuildTree(Guid? parentId) =>
            lookup[parentId]
                .Select(c => new CategoryTreeNodeDto(
                    c.Id,
                    c.Name,
                    c.Description,
                    BuildTree(c.Id)))
                .ToList();

        return BuildTree(null);
    }
}