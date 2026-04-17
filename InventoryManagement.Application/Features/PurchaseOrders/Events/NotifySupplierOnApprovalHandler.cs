using InventoryManagement.Application.Interfaces;
using MediatR;

namespace InventoryManagement.Application.Features.PurchaseOrders.Events;

public class NotifySuuplierOnApprovalHandler(IEmailService emailService)
    : INotificationHandler<PurchaseOrderApprovedEvent>
{
    public async Task Handle(PurchaseOrderApprovedEvent notification, CancellationToken cancellationToken)
    {
        var subject = $"Purchase Order Approved: {notification.PoNumber}";

        var body = $@"
            Hello,
            
            We are pleased to inform you that Purchase Order {notification.PoNumber} has been officially approved.
            Please prepare the shipment at your earliest convenience.
            
            Thank you,
            The Warehouse Team";

        await emailService.SendEmailAsync(notification.SupplierEmail, subject, body, cancellationToken);
    }
}