using Microsoft.EntityFrameworkCore;

namespace FineService.Aggregates.Fine.Entities;

[Owned]
public record LicensePlate(string BaseNumber, uint Region);

public enum FineStatus
{
	Confirmed,
	Suspected,
	Rejected,
	Paid
}