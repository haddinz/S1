using Support.Platform.DTO;

namespace Support.Platform.SycnDataServices.Http.Interfaces;

public interface ICommandDataCLient
{
    Task SendPlatformToComand(PlatformReadDTO plat);
}