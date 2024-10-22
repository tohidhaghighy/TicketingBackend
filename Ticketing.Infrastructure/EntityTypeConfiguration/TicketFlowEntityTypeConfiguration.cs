using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ticketing.Domain.Entities;

namespace Ticketing.Infrastructure.EntityTypeConfiguration;

public class TicketFlowEntityTypeConfiguration: IEntityTypeConfiguration<TicketFlow>
{
    public void Configure(EntityTypeBuilder<TicketFlow> builder)
    {
        builder.HasKey(x => x.Id);
        builder
            .Property(b => b.TicketId)
            .IsRequired();
        builder
            .Property(b => b.UserId)
            .IsRequired();
    }
}