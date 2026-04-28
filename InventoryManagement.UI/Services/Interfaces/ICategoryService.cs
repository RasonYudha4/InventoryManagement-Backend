using InventoryManagement.UI.Models;

namespace InventoryManagement.UI.Services.Interfaces;

public interface ICategoryService
{
    Task<List<CategorySummaryDto>> GetAllCategoriesAsync();
}