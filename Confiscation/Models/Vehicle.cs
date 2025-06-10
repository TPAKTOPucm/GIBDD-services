using Microsoft.EntityFrameworkCore;

namespace ConfiscationService.Models;

[Owned]
public record LicensePlate(string BaseNumber, uint Region);

public class Vehicle
{
	public Guid Id { get; set; }
	public LicensePlate LicensePlate { get; set; }
	public uint UnpaidFineCount { get; set; }
}
