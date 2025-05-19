using Core.Aggregates.Common;
using Core.Aggregates.VehicleRegistration.Entities;

namespace Core.Aggregates.VehicleRegistration.Events;

public record VehicleRegisteredEvent(Guid VehicleId, string Make, string Model, LicensePlate LicensePlate, Guid DriverId,
    FullName DriverFullName, DateTime RegistrationDate) : IDomainEvent;
