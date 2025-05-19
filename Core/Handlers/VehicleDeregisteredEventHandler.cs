using Core.Aggregates.VehicleRegistration.Events;
using MediatR;

namespace Core.Handlers;

public class VehicleDeregisteredEventHandler : INotificationHandler<VehicleDeregisteredEvent>
{
    private readonly ILogger<VehicleDeregisteredEventHandler> _logger;
    public VehicleDeregisteredEventHandler(ILogger<VehicleDeregisteredEventHandler> logger)
    {
        _logger = logger;
    }
    public async Task Handle(VehicleDeregisteredEvent notification, CancellationToken cancellationToken)
    {

        _logger.LogInformation($"Vehicle {notification.LicensePlate.BaseNumber}{notification.LicensePlate.Region} ({notification.Make} {notification.Model}) was deregistered");
    }
}
