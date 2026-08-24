using Support.Auth.Id.Services.Interfaces;

namespace Support.Auth.Id.Services.BackgroundServices;

public class OutboxWorkerService : BackgroundService
{
    private readonly ILogger<OutboxWorkerService> _logger;
    private readonly IServiceScopeFactory _serviceScope;
    private readonly IAuthEmailServices _emailServices;
    private readonly int _batchSize = 10;
    private readonly int _maxRetryCount = 5;
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(10);

    public OutboxWorkerService(ILogger<OutboxWorkerService> logger, IServiceScopeFactory serviceScope, IAuthEmailServices emailServices)
    {
        _logger = logger;
        _serviceScope = serviceScope;
        _emailServices = emailServices;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }
}