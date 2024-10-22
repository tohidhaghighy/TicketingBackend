using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ticketing.Domain.Entities;

namespace Ticketing.Infrastructure.EntityTypeConfiguration;

public class TicketEntityTypeConfiguration: IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.HasKey(x => x.Id);
        builder
            .Property(b => b.UserId)
            .IsRequired();
        builder
            .Property(b => b.CurrentRoleId)
            .IsRequired();
        builder
            .Property(b => b.InsertedRoleId)
            .IsRequired();
        builder.HasOne<Status>()
            .WithMany()
            .HasForeignKey(ticket => ticket.StatusId);
        builder.Property(b => b.Text)
            .HasColumnType("nvarchar(max)");
        builder.Property(b => b.Title)
            .HasColumnType("nvarchar(max)");
    }
}