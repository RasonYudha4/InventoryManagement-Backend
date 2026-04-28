using InventoryManagement.UI.Models;
using InventoryManagement.UI.Models.Requests;

namespace InventoryManagement.UI.Services.Interfaces;

public interface ISupplierService
{
    Task<Guid> CreateSupplierAsync(CreateSupplierRequest request);
    Task<SupplierDetailDto> GetSupplierByIdAsync(Guid id);
    Task<List<SupplierSummaryDto>> GetAllSuppliersAsync();
}