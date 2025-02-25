using Microsoft.Extensions.Hosting;
using Confluent.Kafka;
using System.Text.Json;
using Eshop.Domain;
using Microsoft.Extensions.DependencyInjection;
using Eshop.Persistence.Repositories;

namespace Eshop.KafkaBackgroundService;

public class KafkaBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IConsumer<int, string> _consumer;
    public KafkaBackgroundService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;

        var config = new ConsumerConfig
        {
            GroupId = "order-status-update-group",
            BootstrapServers = Environment.GetEnvironmentVariable("BOOTSTRAP_SERVER") ?? "localhost:29092",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            //SecurityProtocol = SecurityProtocol.SaslSsl,
            EnableAutoCommit = true,
            AutoCommitIntervalMs = 5000
        };
        _consumer = new ConsumerBuilder<int, string>(config).Build();
    }
    public async Task ConsumeAsync(string topic, CancellationToken stoppingToken)
    {
        using (_consumer)
        {
            _consumer.Subscribe(topic);
            while (!stoppingToken.IsCancellationRequested)
            {
                var consumeResult = _consumer.Consume(stoppingToken);
                var orderToUpdate = JsonSerializer.Deserialize<Order>(consumeResult.Message.Value);
                if (orderToUpdate != null)
                {
                    using (var scope = _serviceScopeFactory.CreateScope()) // The background service is singleton while repository/DbContext is scoped.
                    {
                        var repository = scope.ServiceProvider.GetRequiredService<IRepositoryAsync<Order>>();
                        await repository.UpdateAsync(orderToUpdate);
                    }
                }
                else
                {
                    await Task.Delay(1000, stoppingToken);
                }
            }
            _consumer.Close();
        }
        ;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(async () =>
        {
            await ConsumeAsync("order-status-update", stoppingToken);
        }, stoppingToken);
    }
}
