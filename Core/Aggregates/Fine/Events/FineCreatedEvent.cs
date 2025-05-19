using Core.Aggregates.Common;
using Core.Aggregates.VehicleRegistration.Entities;

namespace Core.Aggregates.Fine.Events;

public record FineCreatedEvent(Guid Id, string Reason, DateTime IssueDate, DateTime CreationDate, LicensePlate LicensePlate, decimal Price) : IDomainEvent;
