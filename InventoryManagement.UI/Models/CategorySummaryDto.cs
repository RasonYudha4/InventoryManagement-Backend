namespace InventoryManagement.UI.Models;

public record CategorySummaryDto(
    Guid Id,
    string Name,
    string? Description,
    Guid? ParentCategoryId,
    string? ParentCategoryName
);