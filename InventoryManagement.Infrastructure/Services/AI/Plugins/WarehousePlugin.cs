using System.ComponentModel;
using System.Text.Json;
using Microsoft.EntityFrameworkCore; // CRITICAL for ToListAsync
using InventoryManagement.Infrastructure.Persistence;
using Microsoft.SemanticKernel; // Adjust if your DbContext is elsewhere

namespace InventoryManagement.Infrastructure.Services.AI.Plugins;

public class WarehousePlugin
{
    private readonly ApplicationDbContext _context;

    public WarehousePlugin(ApplicationDbContext context)
    {
        _context = context;
    }

    [KernelFunction("get_low_stock_items")]
    [Description("Gets a list of all products that currently have a stock level below their minimum threshold.")]
    public async Task<string> GetLowStockItemsAsync()
    {
        var items = await _context.Products
            .Where(p => p.ReorderQuantity < 10)
            .Select(p => new 
            {
                p.SKU,
                p.Name,
                p.ReorderQuantity,
                p.SellingPrice
            })
            .ToListAsync();
            
        return JsonSerializer.Serialize(items);
    }
}