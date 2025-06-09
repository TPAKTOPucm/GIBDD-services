using FineService.Aggregates.Common;
using FineService.Aggregates.Fine;
using FineService.Aggregates.Fine.Entities;
using FineService.Mediators;
using Microsoft.EntityFrameworkCore;

namespace FineService.Data;

public class FinesContext: DbContext
{
    private readonly IMediator _mediator;
    public FinesContext(DbContextOptions options, IMediator mediator): base(options)
    {
        _mediator = mediator;
    }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<Fine> Fines { get; set; }
    public DbSet<PaymentReceipt> PaymentReceipts { get; set; }

	public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var result = await base.SaveChangesAsync(cancellationToken);

        await DispatchDomainEventsAsync();

        return result;
    }

    private async Task DispatchDomainEventsAsync()
    {
        var domainEntities = ChangeTracker
            .Entries<IAggregateRoot>()
            .Where(entry => entry.Entity.GetDomainEvents().Any())
            .ToList();

        foreach (var entry in domainEntities)
        {
            var events = entry.Entity.GetDomainEvents();
            entry.Entity.ClearDomainEvents();
            foreach (var @event in events)
            {
                await _mediator.Publish(@event);
            }
        }
    }
}
