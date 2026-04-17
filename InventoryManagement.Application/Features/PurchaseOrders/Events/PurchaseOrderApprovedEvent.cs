using MediatR;

namespace InventoryManagement.Application.Features.PurchaseOrders.Events;

public record PurchaseOrderApprovedEvent(
    Guid PurchaseOrderId,
    string PoNumber,
    string SupplierEmail
) : INotification;