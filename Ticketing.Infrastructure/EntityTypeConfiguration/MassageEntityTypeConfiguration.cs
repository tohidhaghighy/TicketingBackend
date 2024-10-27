using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ticketing.Domain.Entities;

namespace Ticketing.Infrastructure.EntityTypeConfiguration;

public class MassageEntityTypeConfiguration: IEntityTypeConfiguration<Massage>
{
    public void Configure(EntityTypeBuilder<Massage> builder)
    {
        builder.HasKey(x => x.Id);
        builder
            .Property(b => b.TicketId)
            .IsRequired();
        builder
            .Property(b => b.InsertDate)
            .IsRequired();
        builder.HasOne<Ticket>()
            .WithMany()
            .HasForeignKey(massage => massage.TicketId);
        builder.Property(b => b.Text)
            .HasColumnType("nvarchar(max)");
        builder.Property(b => b.FilePath)
            .HasColumnType("nvarchar(max)");
    }
}