using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Support.Auth.Id.Domain.Entity;

namespace Support.Auth.Id.Repository.Data.Configuration;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.Property(x => x.Email).HasMaxLength(100).IsRequired();
        builder.Property(x => x.ImmutableEmail).HasMaxLength(100).IsRequired();
        builder.Property(x => x.FullName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.UserName).HasMaxLength(50).IsRequired();
        builder.Property(x => x.PasswordHash).HasMaxLength(500).IsRequired();
        builder.Property(x => x.PhoneNumber).HasMaxLength(30);

        // Index for performance query
        builder.HasIndex(x => x.UserName).IsUnique();
        builder.HasIndex(x => x.Email).IsUnique();

        // using OwnsOne to tell Efcore that not can be relation
        // because its value object
        builder.OwnsOne(
            x => x.EmailVerification,
            email =>
            {
                email
                    .Property(x => x.Token)
                    .HasMaxLength(225)
                    .HasColumnName("EmailVerificationToken");
                email.Property(x => x.ExpiresAt).HasColumnName("EmailVerificationExpiresAt");
                email.Property(x => x.VerifiedAt).HasColumnName("EmailVerifiedAt");
            }
        );

        builder.OwnsOne(
            x => x.PasswordReset,
            passwordReset =>
            {
                passwordReset
                    .Property(x => x.Token)
                    .HasMaxLength(225)
                    .HasColumnName("PasswordResetToken");
                passwordReset.Property(x => x.ExpiresAt).HasColumnName("PasswordResetExpiresAt");
                passwordReset.Property(x => x.UsedAt).HasColumnName("PasswordResetUsedAt");
            }
        );

        // relation many to many just one side is enought. User and Role
        builder.HasMany(x => x.Roles).WithMany(x => x.Users);

        builder
            .HasMany(x => x.RefreshTokens)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
