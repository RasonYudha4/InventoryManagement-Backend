using InventoryManagement.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Infrastructure.Services;

public class MockEmailService(ILogger<MockEmailService> logger) : IEmailService
{
    public Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        logger.LogWarning("========== EMAIL INTERCEPTED ==========");
        logger.LogWarning("TO: {To}", to);
        logger.LogWarning("SUBJECT: {Subject}", subject);
        logger.LogWarning("BODY: {Body}", body);
        logger.LogWarning("=======================================");

        return Task.CompletedTask;
    }
}