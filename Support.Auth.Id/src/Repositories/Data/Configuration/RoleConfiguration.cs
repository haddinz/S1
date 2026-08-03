using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Support.Auth.Id.Models.Entity;

namespace Support.Auth.Id.Repository.Data.Configuration;

public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");

        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.Property(x => x.RoleType).HasMaxLength(50).IsRequired();
        builder.Property(x => x.RoleName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(100);

        builder.HasIndex(x => x.RoleName).IsUnique();
    }
}
