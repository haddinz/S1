using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Support.Platform.DTO;

namespace Support.Platform.AsyncDataServices;

public class MessageBusClient : IMessageBusClient
{
    private readonly ILogger<MessageBusClient> _logger;
    private readonly IConfiguration _configuration;
    private IConnection? _connection = null;
    private IChannel? _channel = null;

    // private readonly TaskCompletionSource<bool> _ready = new();

    public MessageBusClient(ILogger<MessageBusClient> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task InitializeRabbitMQAsync()
    {
        _logger.LogInformation("--> Trying to Connect to Message Bus");

        ConnectionFactory factory = new()
        {
            HostName = _configuration["RabbitMQHost"] ?? "localhost",
            Port = int.Parse(_configuration["RabbitMQPort"]!),
        };

        try
        {
            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await _channel.ExchangeDeclareAsync(exchange: "trigger", ExchangeType.Fanout);
            _connection.ConnectionShutdownAsync += RabbitMQConnectionShutdown;

            _logger.LogInformation("--> Connected to Message Bus");
            // _ready.TrySetResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError("--> Could NoConnect to The Message {ex.Message}", ex.Message);
        }
    }

    public async Task PublishNewPlatform(PlatformPublishedDto dto)
    {
        // Always care about method you call Lazy Init
        // await InitializeRabbitMQAsync();

        string message = JsonSerializer.Serialize(dto);

        // await _ready.Task;

        if (_connection!.IsOpen)
        {
            _logger.LogInformation("--> RabbitMQ Connection Open, sending message....");
            await SendMessage(message);
        }
        else
        {
            _logger.LogWarning("--> RabbitMQ Connection Closed");
        }
    }

    private async Task SendMessage(string message)
    {
        byte[] body = Encoding.UTF8.GetBytes(message);
        BasicProperties props = new();

        await _channel!.BasicPublishAsync(
            exchange: "trigger",
            routingKey: "",
            mandatory: false,
            basicProperties: props,
            body: body
        );

        _logger.LogInformation("--> We have Sent a Message = {message}", message);
    }

    public void Dispose()
    {
        _logger.LogInformation("--> Message Bus Disposed");

        if (_channel!.IsOpen)
        {
            _channel.CloseAsync();
            _connection?.CloseAsync();
        }
    }

    private Task RabbitMQConnectionShutdown(Object sender, ShutdownEventArgs e)
    {
        _logger.LogInformation("--> RabbitMQ Connection Shutdown");
        return Task.CompletedTask;
    }
}
