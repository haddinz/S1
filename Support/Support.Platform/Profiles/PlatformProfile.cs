using AutoMapper;
using Support.Platform.DTO;
using Support.Platform.Models;

namespace Support.Platform.Profiles;

public class PlatformProfile : Profile
{
    public PlatformProfile()
    {
        // source => target
        CreateMap<PlatformModel, PlatformReadDTO>();
        CreateMap<PlatformCreateDTO, PlatformModel>();

        CreateMap<PlatformReadDTO, PlatformPublishedDto>();

        // Its because different type Id 
        // PlatformModels = GUID
        // PlatformGrpc = string
        CreateMap<PlatformModel, GrpcPlatformModels>()
            .ForMember(dest => dest.PlatformId, opt => opt.MapFrom(src => src.Id.ToString()))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Publisher, opt => opt.MapFrom(src => src.Publisher));

        CreateMap<GrpcPlatformModels, PlatformModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.Parse(src.PlatformId)))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Publisher, opt => opt.MapFrom(src => src.Publisher));
    }
}
