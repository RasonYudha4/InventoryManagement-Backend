using InventoryManagement.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Application.Features.Stock.Queries.GetLowStock;

public class GetLowStockQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetLowStockQuery, List<LowStockItemDto>>
{
    public async Task<List<LowStockItemDto>> Handle(GetLowStockQuery request, CancellationToken cancellationToken)
    {
        return await context.StockLevels
            .Include(s => s.Product)
            .Where(s => s.Quantity <= s.Product.ReorderPoint)
            .Select(s => new LowStockItemDto(
                s.ProductId,
                s.Product.Name,
                s.Quantity,
                s.Product.ReorderPoint))
            .ToListAsync(cancellationToken);
    }
}