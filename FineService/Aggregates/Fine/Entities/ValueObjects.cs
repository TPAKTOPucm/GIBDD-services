using Microsoft.EntityFrameworkCore;

namespace FineService.Aggregates.Fine.Entities;

[Owned]
public record LicensePlate(string BaseNumber, uint Region);

public enum FineStatus
{
	Suspected,
	Confirmed,
	Rejected,
	Paid
}