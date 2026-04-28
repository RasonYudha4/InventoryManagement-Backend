using InventoryManagement.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Application.Features.Products.Queries.GetAllProducts;

public class GetAllProductsQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetAllProductsQuery, List<ProductSummaryDto>>
{
    public async Task<List<ProductSummaryDto>> Handle(
        GetAllProductsQuery request,
        CancellationToken cancellationToken)
    {
        return await context.Products
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .Where(p => p.IsActive)
            .OrderBy(p => p.Name)
            .Select(p => new ProductSummaryDto(
                p.Id,
                p.SKU,
                p.Name,
                p.SellingPrice,
                p.ReorderPoint,
                p.Category.Name,
                p.Supplier.Name,
                p.IsActive))
            .ToListAsync(cancellationToken);
    }
}