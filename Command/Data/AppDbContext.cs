using Command.Models;
using Microsoft.EntityFrameworkCore;

namespace Command.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> opt)
        : base(opt) { }

    public DbSet<Platform> Platforms { get; set; }
    public DbSet<Commands> Commands { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Platform>()
            .HasMany(p => p.Commands)
            .WithOne(p => p.Platform)
            .HasForeignKey(p => p.PlatformId);

        modelBuilder
            .Entity<Commands>()
            .HasOne(c => c.Platform)
            .WithMany(c => c.Commands)
            .HasForeignKey(c => c.PlatformId);
    }
}
