using System.Net.Http.Json;
using InventoryManagement.UI.Models;
using InventoryManagement.UI.Services.Interfaces;

namespace InventoryManagement.UI.Services;

public class CategoryService(HttpClient http) : ICategoryService
{
    public async Task<List<CategorySummaryDto>> GetAllCategoriesAsync()
    {
        var result = await http.GetFromJsonAsync<List<CategorySummaryDto>>("api/category");
        return result ?? [];
    }
}