using InventoryManagement.UI.Models;
using InventoryManagement.UI.Models.Requests;

namespace InventoryManagement.UI.Services.Interfaces;

public interface IProductService
{
    Task<Guid> CreateProductAsync(CreateProductRequest request);
    Task<ProductDetailDto> GetProductByIdAsync(Guid id);
    Task<List<ProductSummaryDto>> GetAllProductsAsync();
}