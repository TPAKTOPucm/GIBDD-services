using Microsoft.EntityFrameworkCore;

namespace Core.Aggregates.VehicleRegistration.Entities;

[Owned]
public record LicensePlate(string BaseNumber, uint Region);
