namespace InventoryManagement.UI.Models;

public record SupplierSummaryDto(
    Guid Id,
    string Name,
    string ContactName,
    string Email,
    string? Phone,
    bool IsActive
);