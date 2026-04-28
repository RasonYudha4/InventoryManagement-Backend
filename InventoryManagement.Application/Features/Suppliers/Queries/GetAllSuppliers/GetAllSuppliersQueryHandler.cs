using InventoryManagement.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Application.Features.Suppliers.Queries.GetAllSuppliers;

public class GetAllSuppliersQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetAllSuppliersQuery, List<SupplierSummaryDto>>
{
    public async Task<List<SupplierSummaryDto>> Handle(
        GetAllSuppliersQuery request,
        CancellationToken cancellationToken)
    {
        return await context.Suppliers
            .Where(s => s.IsActive)
            .OrderBy(s => s.Name)
            .Select(s => new SupplierSummaryDto(
                s.Id,
                s.Name,
                s.ContactName,
                s.Email,
                s.Phone,
                s.IsActive))
            .ToListAsync(cancellationToken);
    }
}