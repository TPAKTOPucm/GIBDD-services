using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ConfiscationService.Models;

[Owned]
public record LicensePlate(string BaseNumber, uint Region);

public class Vehicle
{
	[Key]
	public LicensePlate LicensePlate { get; set; }
	public uint UnpaidFineCount { get; set; }
}
