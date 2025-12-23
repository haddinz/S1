using Support.Platform.DTO;

namespace Support.Platform.AsyncDataServices;

public interface IMessageBusClient
{
    Task InitializeRabbitMQAsync();
    Task PublishNewPlatform(PlatformPublishedDto dto);
    void Dispose();
}
