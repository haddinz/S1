using Support.Platform.Data;
using Support.Platform.Interfaces;
using Support.Platform.Models;

namespace Support.Platform.Repository;

public class PlatformRepo(AppDBContext context) : IPlatformRepo
{
    private readonly AppDBContext _context = context;

    public void CreatePlatform(PlatformModel platform)
    {
        ArgumentNullException.ThrowIfNull(platform);

        _context.Platforms.Add(platform);
    }

    public IEnumerable<PlatformModel> GetAllPlatforms()
    {
        return [.. _context.Platforms];
    }

    public PlatformModel GetPlatformById(Guid id)
    {
        return _context.Platforms.FirstOrDefault(p => p.Id == id)!;
    }

    public bool SaveChanges()
    {
        return _context.SaveChanges() > 0;
    }
}