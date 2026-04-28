using MediatR;

namespace InventoryManagement.Application.Features.Suppliers.Queries.GetAllSuppliers;

public record SupplierSummaryDto(
    Guid Id,
    string Name,
    string ContactName,
    string Email,
    string? Phone,
    bool IsActive
);

public record GetAllSuppliersQuery : IRequest<List<SupplierSummaryDto>>;