using Core.Aggregates.VehicleRegistration.Events;

namespace Core.Handlers;

public class VehicleRegisteredEventHandler
{
    private readonly ILogger<VehicleRegisteredEventHandler> _logger;
    public VehicleRegisteredEventHandler(ILogger<VehicleRegisteredEventHandler> logger)
    {
        _logger = logger;
    }
    public async Task Handle(VehicleRegisteredEvent notification, CancellationToken cancellationToken)
    {

        _logger.LogInformation($"Vehicle {notification.LicensePlate.BaseNumber}{notification.LicensePlate.Region} ({notification.Make} {notification.Model}) was registered. Owner is {notification.DriverFullName.FirstName[0]}. {notification.DriverFullName.LastName}");
    }
}
