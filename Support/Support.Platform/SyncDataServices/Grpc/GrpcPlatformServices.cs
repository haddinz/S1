using AutoMapper;
using Grpc.Core;
using Support.Platform.Interfaces;
using Support.Platform.Models;

namespace Support.Platform.SycnDataServices.Grpc;

public class GrpcPlatformService : GrpcPlatform.GrpcPlatformBase
{
    private readonly IMapper _mapper;
    private readonly IPlatformRepo _repo;

    public GrpcPlatformService(IMapper mapper, IPlatformRepo repo)
    {
        _mapper = mapper;
        _repo = repo;
    }

    public override Task<PlatformResponse> GetAllPlatforms(
        GetAllRequest request,
        ServerCallContext context
    )
    {
        PlatformResponse response = new();
        IEnumerable<PlatformModel> platforms = _repo.GetAllPlatforms();

        foreach (PlatformModel plat in platforms)
        {
            response.Platform.Add(_mapper.Map<GrpcPlatformModels>(plat));
        }

        return Task.FromResult(response);
    }
}
