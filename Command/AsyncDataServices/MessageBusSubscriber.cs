using System.Text;
using System.Threading.Tasks;
using Command.EventProcessing.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Command.AsyncDataServices;

public class MessageBusSubscriber : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger _logger;
    private readonly IEventProcessor _eventProcessor;
    private IConnection? _connection;
    private IChannel? _channel;
    private string? _queueName;

    public MessageBusSubscriber(
        IConfiguration configuration,
        ILogger<MessageBusSubscriber> logger,
        IEventProcessor eventProcessor
    )
    {
        _configuration = configuration;
        _logger = logger;
        _eventProcessor = eventProcessor;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("--> Start Listening to the Message Bus");

            ConnectionFactory factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQHost"]!,
                Port = int.Parse(_configuration["RabbitMQPort"]!),
            };

            _connection = await factory.CreateConnectionAsync();

            _channel = await _connection.CreateChannelAsync();
            await _channel.ExchangeDeclareAsync(exchange: "trigger", type: ExchangeType.Fanout);

            // QueueDeclareOk queueDeclareOk = await _channel!.QueueDeclareAsync(
            //     queue: "",
            //     durable: false,
            //     exclusive: true, // if its true meke it private
            //     autoDelete: true
            // );

            QueueDeclareOk queueDeclareOk = await _channel!.QueueDeclareAsync(
                queue: "",
                durable: true,
                exclusive: false,
                autoDelete: false
            );

            _queueName = queueDeclareOk.QueueName;

            await _channel!.QueueBindAsync(
                queue: _queueName,
                exchange: "trigger",
                routingKey: "",
                cancellationToken: cancellationToken
            );

            _logger.LogInformation("--> Success Listening to the Message Bus");

            _connection.ConnectionShutdownAsync += RabbitMQConnectionShutdown;
            await base.StartAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogInformation("--> Failed Listening to the Message Bus {ex}", ex);
        }
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("--> ExecuteAsync Message Bus Has Started Running");
        cancellationToken.ThrowIfCancellationRequested();

        AsyncEventingBasicConsumer consumer = new AsyncEventingBasicConsumer(_channel!);
        consumer.ReceivedAsync += async (ModuleHandle, ea) =>
        {
            _logger.LogInformation("--> Event Received");

            var body = ea.Body;
            string notificationMessage = Encoding.UTF8.GetString(body.ToArray());

            _eventProcessor.ProcessEvent(notificationMessage);
        };

        await _channel!.BasicConsumeAsync(queue: _queueName!, autoAck: true, consumer: consumer);

        _logger.LogInformation("--> ExecuteAsync Message Bus, Awaiting Cancelation .....");
        await Task.Delay(Timeout.Infinite, cancellationToken);
    }

    private Task RabbitMQConnectionShutdown(object? sender, ShutdownEventArgs e)
    {
        _logger.LogInformation("--> Connection Shutdown: {Reason}", e.ReplyText);
        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        if (_channel!.IsOpen)
        {
            _channel.CloseAsync();
        }
        base.Dispose();
    }
}
