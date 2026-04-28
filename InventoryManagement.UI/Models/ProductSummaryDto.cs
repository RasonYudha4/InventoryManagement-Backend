namespace InventoryManagement.UI.Models;

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
