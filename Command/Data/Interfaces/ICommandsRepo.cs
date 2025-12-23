using System.Collections;
using Command.Models;

namespace Command.Data.Interfaces;

public interface ICommandsRepo
{
    bool SaveChanges();

    IEnumerable<Platform> GetAllPlatform();
    void CreatePlatform(Platform plat);
    bool PlatformExist(Guid platformId);
    bool ExternalPlatformExist(Guid externalPlatformId);

    IEnumerable<Commands> GetCommandsForPlatform(Guid platformId);
    Commands GetCommand(Guid platformId, Guid commandsId);
    void CreateCommand(Guid platformId, Commands commands);
}