using System.Net.Http.Json;
using InventoryManagement.UI.Models;
using InventoryManagement.UI.Models.Requests;
using InventoryManagement.UI.Services.Interfaces;

namespace InventoryManagement.UI.Services;

public class SupplierService(HttpClient http) : ISupplierService
{
    public async Task<Guid> CreateSupplierAsync(CreateSupplierRequest request)
    {
        var response = await http.PostAsJsonAsync("api/supplier", request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ApiIdResponse>();
        return result!.Id;
    }

    public async Task<SupplierDetailDto> GetSupplierByIdAsync(Guid id)
    {
        var result = await http.GetFromJsonAsync<SupplierDetailDto>($"api/supplier/{id}");
        return result!;
    }
    public async Task<List<SupplierSummaryDto>> GetAllSuppliersAsync()
    {
        var result = await http.GetFromJsonAsync<List<SupplierSummaryDto>>("api/supplier");
        return result ?? [];
    }
}