using FineService.Aggregates.Common;
using FineService.Aggregates.Fine.Entities;

namespace FineService.Aggregates.Fine.Events;

public record FineStatusChangedEvent(Guid Id, string Reason, DateTime IssueDate, DateTime CreationDate, LicensePlate LicensePlate, decimal Price, FineStatus FineStatus) : IDomainEvent;

public record FineRejectedEvent(Guid Id, string Reason, DateTime IssueDate, LicensePlate LicensePlate, FineStatus FineStatus = FineStatus.Rejected) : IDomainEvent;