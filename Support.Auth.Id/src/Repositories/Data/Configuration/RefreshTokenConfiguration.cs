using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Support.Auth.Id.Models.Entity;

namespace Support.Auth.Id.Repository.Data.Configuration;

public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");

        builder.Property(x => x.Token).HasMaxLength(500).IsRequired();
        builder.Property(x => x.CreatedByIp).HasMaxLength(100).IsRequired();
        builder.Property(x => x.RevokedByIp).HasMaxLength(100);
        builder.Property(x => x.ReplacedByToken).HasMaxLength(500);

        builder.HasIndex(x => x.Token).IsUnique();
    }
}
