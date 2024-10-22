using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ticketing.Domain.Entities;

namespace Ticketing.Infrastructure.EntityTypeConfiguration;

public class StatusEntityTypeConfiguration: IEntityTypeConfiguration<Status>
{
    public void Configure(EntityTypeBuilder<Status> builder)
    {
        builder.HasKey(x => x.Id);
        builder
            .Property(b => b.Name)
            .IsRequired();
        builder.Property(b => b.Name)
            .HasColumnType("nvarchar(max)");
    }
}