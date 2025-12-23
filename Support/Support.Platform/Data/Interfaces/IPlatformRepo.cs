using Support.Platform.Models;

namespace Support.Platform.Interfaces;

public interface IPlatformRepo
{
    bool SaveChanges();

    IEnumerable<PlatformModel> GetAllPlatforms();
    PlatformModel GetPlatformById(Guid id);
    void CreatePlatform(PlatformModel plat);
}