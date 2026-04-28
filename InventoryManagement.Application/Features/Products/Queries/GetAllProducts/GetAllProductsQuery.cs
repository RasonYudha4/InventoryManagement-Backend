using MediatR;

namespace InventoryManagement.Application.Features.Products.Queries.GetAllProducts;

public record ProductSummaryDto(
    Guid Id,
    string SKU,
    string Name,
    decimal SellingPrice,
    int ReorderPoint,
    string CategoryName,
    string SupplierName,
    bool IsActive
);

public record GetAllProductsQuery : IRequest<List<ProductSummaryDto>>;