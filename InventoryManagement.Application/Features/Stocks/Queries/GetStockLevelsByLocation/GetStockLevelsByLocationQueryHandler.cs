using InventoryManagement.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Application.Features.Stock.Queries.GetStockLevelsByLocation;

public class GetStockLevelsByLocationQueryHandler(IApplicationDbContext context) 
: IRequestHandler<GetStockLevelsByLocationQuery, List<StockLevelDto>>
{
    public async Task<List<StockLevelDto>> Handle(GetStockLevelsByLocationQuery request, CancellationToken cancellationToken)
    {
        var locationExists = await context.Locations
            .AnyAsync(l => l.Id == request.LocationId, cancellationToken);

        if (!locationExists)
        {
            throw new Exception($"Location with ID {request.LocationId} not found.");
        }

        var stockLevels = await context.StockLevels
            .AsNoTracking()
            .Include(sl => sl.Product)
            .Where(sl => sl.LocationId == request.LocationId)
            .Select(sl => new StockLevelDto(
                sl.ProductId,
                sl.Product.SKU,
                sl.Product.Name,
                sl.Quantity,
                sl.AllocatedQuantity,
                sl.AvailableQuantity
            )).ToListAsync(cancellationToken);

        return stockLevels;
    }
}