using Core.Aggregates.Fine.Events;
using MediatR;

namespace Core.Handlers;

public class FineCreatedEventHandler : INotificationHandler<FineCreatedEvent>
{
    private readonly ILogger<FineCreatedEventHandler> _logger;
    public FineCreatedEventHandler(ILogger<FineCreatedEventHandler> logger)
    {
        _logger = logger;
    }
    public Task Handle(FineCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"A {notification.Reason} fine has been issued for the {notification.LicensePlate.BaseNumber}{notification.LicensePlate.Region} car. Price is {notification.Price}");
        return Task.CompletedTask;
    }
}
