using MediatR;

namespace InventoryManagement.Application.Features.Stock.Queries.GetStockLevelsByLocation;

public record GetStockLevelsByLocationQuery(Guid LocationId) : IRequest<List<StockLevelDto>>;