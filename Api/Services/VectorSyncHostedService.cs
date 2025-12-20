using Agentic_Rentify.Infragentic.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Agentic_Rentify.Api.Services;

public class VectorSyncHostedService(IServiceScopeFactory scopeFactory, ILogger<VectorSyncHostedService> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            logger.LogInformation("VectorSyncHostedService starting one-time sync...");
            using var scope = scopeFactory.CreateScope();
            var dataSyncService = scope.ServiceProvider.GetRequiredService<DataSyncService>();
            await dataSyncService.SyncAsync("rentify_memory");
            logger.LogInformation("VectorSyncHostedService completed initial sync.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "VectorSyncHostedService encountered an error during initial sync.");
        }
    }
}
