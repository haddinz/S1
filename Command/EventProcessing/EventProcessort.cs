using System.Text.Json;
using AutoMapper;
using Command.Data.Interfaces;
using Command.Dtos;
using Command.EventProcessing.Interfaces;
using Command.Models;
using Command.Models.Enum;

namespace Command.EventProcessing;

public class EventProcessor : IEventProcessor
{
    private readonly ILogger _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMapper _mapper;

    public EventProcessor(
        ILogger<EventProcessor> logger,
        IServiceScopeFactory scopeFactory,
        IMapper mapper
    )
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _mapper = mapper;
    }

    public void ProcessEvent(string message)
    {
        EventType eventType = DeterminateEvent(message);

        switch (eventType)
        {
            case EventType.PlatfoemPublished:
                AddPlatform(message);
                break;
            default:
                break;
        }
    }

    private EventType DeterminateEvent(string notificationMessage)
    {
        _logger.LogInformation("--> Determining Event");

        GenericEventDto eventType = JsonSerializer.Deserialize<GenericEventDto>(
            notificationMessage
        )!;

        switch (eventType.Event)
        {
            case Published.Platform_Published:
                _logger.LogInformation("--> Platform Published Event Detected");
                return EventType.PlatfoemPublished;
            default:
                _logger.LogWarning("--> Could Not Determmine the Event Type");
                return EventType.Undertermined;
        }
    }

    private void AddPlatform(string platformPublishMessage)
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var repo = scope.ServiceProvider.GetRequiredService<ICommandsRepo>();

            PlatformPublishedDto platformPublishDto =
                JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishMessage)!;

            try
            {
                Platform plat = _mapper.Map<Platform>(platformPublishDto);

                if (!repo.ExternalPlatformExist(plat.ExternalId))
                {
                    repo.CreatePlatform(plat);
                    repo.SaveChanges();
                    _logger.LogInformation("--> {platName} Platform Added", plat.Name);
                }
                else
                {
                    _logger.LogWarning("--> {platName} Platform Already Exist", plat.Name);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("--> Could Not AddPlatform to DB {ex}", ex.Message);
            }
        }
    }
}
