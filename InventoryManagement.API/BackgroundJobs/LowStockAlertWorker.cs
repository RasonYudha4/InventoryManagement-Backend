using InventoryManagement.Application.Features.Stock.Queries.GetLowStock;
using InventoryManagement.Application.Interfaces;
using MediatR;

namespace InventoryManagement.API.BackgroundJobs;

public class LowStockAlertWorker(
    IServiceProvider serviceProvider,
    ILogger<LowStockAlertWorker> logger
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Low Stock Alert Worker is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                logger.LogInformation("Worker waking up to scan inventory...");

                using (var scope = serviceProvider.CreateScope())
                {
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                    var lowStockItems = await mediator.Send(new GetLowStockQuery(), stoppingToken);

                    if (lowStockItems.Any())
                    {
                        logger.LogWarning("Found {Count} items below reorder point! Sending alert...", lowStockItems.Count);

                        var body = "The following items urgently need to be reordered:\n";
                        foreach (var item in lowStockItems)
                        {
                            body += $"- {item.ProductName}: {item.CurrentQuantity} left (Threshold: {item.ReorderPoint})\n";
                        }

                        await emailService.SendEmailAsync("procurement@test.com", "URGENT: Low Stock Alert", body, stoppingToken);
                    }
                    else
                    {
                        logger.LogInformation("Inventory levels look good.");
                    }
                }
            } 
            catch (Exception e)
            {
                logger.LogError(e, "An error occured while scanning for low stock.");
            }

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }
}