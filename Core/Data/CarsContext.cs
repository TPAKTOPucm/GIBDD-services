using Core.Aggregates.Common;
using Core.Aggregates.Fine;
using Core.Aggregates.Fine.Entities;
using Core.Aggregates.VehicleRegistration;
using Core.Aggregates.VehicleRegistration.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Data;

public class CarsContext: DbContext
{
    private readonly IMediator _mediator;
    public CarsContext(DbContextOptions options, IMediator mediator): base(options)
    {
        _mediator = mediator;
    }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<Driver> Drivers { get; set; }
    public DbSet<Fine> Fines { get; set; }
    public DbSet<PaymentReceipt> PaymentReceipts { get; set; }
    public DbSet<VehicleRegistration> VehicleRegistrations { get; set; }

	public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        var result = await base.SaveChangesAsync(cancellationToken);

        await DispatchDomainEventsAsync(cancellationToken);

        return result;
    }

    private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken)
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
                await _mediator.Publish(@event, cancellationToken);
            }
        }
    }
}
