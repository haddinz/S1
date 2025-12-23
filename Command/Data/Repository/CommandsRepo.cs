using Command.Data.Interfaces;
using Command.Models;

namespace Command.Data.Repository;

public class CommandsRepo(AppDbContext context) : ICommandsRepo
{
    private readonly AppDbContext _context = context;

    public void CreateCommand(Guid platformId, Commands commands)
    {
        ArgumentNullException.ThrowIfNull(commands);

        commands.PlatformId = platformId;
        _context.Commands.Add(commands);
    }

    public void CreatePlatform(Platform plat)
    {
        ArgumentNullException.ThrowIfNull(plat);

        _context.Platforms.Add(plat);
    }

    public bool ExternalPlatformExist(Guid externalPlatformId)
    {
        return _context.Platforms.Any(p => p.ExternalId == externalPlatformId);
    }

    public IEnumerable<Platform> GetAllPlatform()
    {
        return _context.Platforms.ToList();
    }

    public Commands GetCommand(Guid platformId, Guid commandsId)
    {
        return _context
            .Commands.Where(c => c.PlatformId == platformId && c.Id == commandsId)
            .FirstOrDefault()!;
    }

    public IEnumerable<Commands> GetCommandsForPlatform(Guid platformId)
    {
        return _context
            .Commands.Where(c => c.PlatformId == platformId)
            .OrderBy(c => c.Platform!.Name);
    }

    public bool PlatformExist(Guid platformId)
    {
        return _context.Platforms.Any(p => p.Id == platformId);
    }

    public bool SaveChanges()
    {
        return _context.SaveChanges() >= 0;
    }
}
