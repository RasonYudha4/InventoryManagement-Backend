using System.Net.Http.Json;
using InventoryManagement.UI.Models;
using InventoryManagement.UI.Models.Requests;
using InventoryManagement.UI.Services.Interfaces;

namespace InventoryManagement.UI.Services;

public class ProductService(HttpClient http) : IProductService
{
    public async Task<Guid> CreateProductAsync(CreateProductRequest request)
    {
        var response = await http.PostAsJsonAsync("api/product", request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ApiIdResponse>();
        return result!.Id;
    }

    public async Task<ProductDetailDto> GetProductByIdAsync(Guid id)
    {
        var product = await http.GetFromJsonAsync<ProductDetailDto>($"api/product/{id}");
        return product!;
    }

    public async Task<List<ProductSummaryDto>> GetAllProductsAsync()
    {
        var result = await http.GetFromJsonAsync<List<ProductSummaryDto>>("api/product");
        return result ?? [];
    }
}