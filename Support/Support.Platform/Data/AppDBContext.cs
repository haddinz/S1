using Microsoft.EntityFrameworkCore;
using Support.Platform.Models;

namespace Support.Platform.Data;

public class AppDBContext : DbContext
{
    public AppDBContext(DbContextOptions<AppDBContext> opt)
        : base(opt) { }

    public DbSet<PlatformModel> Platforms { get; set; }
}
