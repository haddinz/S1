using AutoMapper;
using Command.Dtos;
using Command.Models;
using Support.Platform;

namespace Command.Profiles;

public class CommandsProfile : Profile
{
    public CommandsProfile()
    {
        // source => target
        CreateMap<Platform, PlatformReadDto>();
        CreateMap<CommandsCreateDto, Commands>();
        CreateMap<Commands, CommandsReadDto>();

        CreateMap<PlatformPublishedDto, Platform>()
            .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.Id));

        CreateMap<GrpcPlatformModels, Platform>()
            .ForMember(
                dest => dest.ExternalId,
                opt => opt.MapFrom(src => Guid.Parse(src.PlatformId))
            )
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Commands, opt => opt.Ignore());
    }
}
