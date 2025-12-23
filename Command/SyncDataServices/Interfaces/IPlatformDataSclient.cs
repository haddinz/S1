using Command.Models;

namespace Command.SyncDataServices;

public interface IPlatformDataClient
{
    IEnumerable<Platform> ReturnAllPlatforms();
}
