namespace InventoryManagement.API.Models;

public record ReceiveStockRequest(
    Guid ProductId,
    Guid LocationId,
    int Quantity,
    string? ReferenceNumber,
    string? Notes
);