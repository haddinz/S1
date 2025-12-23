using AutoMapper;
using Command.Models;
using Grpc.Net.Client;
using Support.Platform;

namespace Command.SyncDataServices.Grpc;

public class PlatformDataClient : IPlatformDataClient
{
    private readonly ILogger _logger;
    private readonly IConfiguration _config;
    private readonly IMapper _mapper;

    public PlatformDataClient(
        ILogger<PlatformDataClient> logger,
        IConfiguration config,
        IMapper mapper
    )
    {
        _logger = logger;
        _config = config;
        _mapper = mapper;
    }

    public IEnumerable<Platform> ReturnAllPlatforms()
    {
        _logger.LogInformation("--> Hit Grpc Platform Client at {con}", _config["GrpcPlatform"]);

        GrpcChannel channel = GrpcChannel.ForAddress(_config["GrpcPlatform"]!);
        GrpcPlatform.GrpcPlatformClient client = new(channel);

        GetAllRequest request = new();

        try
        {
            PlatformResponse reply = client.GetAllPlatforms(request);
            return _mapper.Map<IEnumerable<Platform>>(reply.Platform);
        }
        catch (Exception ex)
        {
            _logger.LogError("Could not cal  Grpc Server {ex}", ex.Message);
            return null!;
        }
    }
}
