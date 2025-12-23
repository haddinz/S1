namespace Support.Platform.AsyncDataServices;

public class RabbitMqInitializer : IHostedService
{
    private readonly IMessageBusClient _messageBusClient;

    public RabbitMqInitializer(IMessageBusClient messageBusClient)
    {
        _messageBusClient = messageBusClient;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _messageBusClient.InitializeRabbitMQAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _messageBusClient.Dispose();

        return Task.CompletedTask;
    }
}
