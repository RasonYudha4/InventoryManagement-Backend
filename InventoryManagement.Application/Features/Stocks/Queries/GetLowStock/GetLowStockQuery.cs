using MediatR;

namespace InventoryManagement.Application.Features.Stock.Queries.GetLowStock;

public record LowStockItemDto(Guid ProductId, string ProductName, int CurrentQuantity, int ReorderPoint);

public record GetLowStockQuery() : IRequest<List<LowStockItemDto>>;