using Eshop.Domain;
using Eshop.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Eshop.Persistence;

public class PaymentProcessingService : BackgroundService
{
    private readonly IProcessingQueue<Order> _processingQueue;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public PaymentProcessingService(IProcessingQueue<Order> queue, IServiceScopeFactory service)
    {
        _processingQueue = queue;
        _serviceScopeFactory = service;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var updatedOrder = _processingQueue.Fetch();
            if (updatedOrder != null)
            {
                using (var scope = _serviceScopeFactory.CreateScope()) // The background service is singleton while repository/DbContext is scoped.
                {
                    var repository = scope.ServiceProvider.GetRequiredService<IRepositoryAsync<Order>>();
                    await repository.UpdateAsync(updatedOrder);
                }
            }
            else
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
