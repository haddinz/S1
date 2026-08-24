using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Support.Auth.Id.Constans;
using Support.Auth.Id.Domain.Entity;

namespace Support.Auth.Id.Repository.Data.Configuration;

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessage");

        builder.Property(x => x.Type).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Payload).HasColumnType("jsonb").IsRequired();
        builder.Property(x => x.Status).HasMaxLength(20).HasDefaultValue(Message.Status.Pending).IsRequired();
        builder.Property(x => x.RetryCount).HasDefaultValue(0).IsRequired();
        builder.Property(x => x.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP").IsRequired();

        // Indexing or Composite Index (Index gabungan)
        builder
            .HasIndex(x => x.Type)
            .HasDatabaseName("IDX_OutboxMessages_Type");
        builder
            .HasIndex(x => new {x.Status, x.CreatedAt})
            .HasDatabaseName("IDX_OutboxMessages_Status_CreatedAt");
    }
}