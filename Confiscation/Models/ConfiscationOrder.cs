using Microsoft.EntityFrameworkCore;

namespace ConfiscationService.Models;

public enum ConfiscationReason
{
	UnpaidFines,
	GrossOffense,
	Other
}

[Owned]
public record Address(uint ZipCode, string City, string CityType, string Street, uint? Home);

public class ConfiscationOrder
{
	public Guid Id { get; init; }
	public LicensePlate LicensePlate { get; set; }
	public ConfiscationReason Reason { get; set; }
	public string Comment { get; set; }
	public Address? ImpoundYardAddress { get; set; }
	public bool IsReturned { get; set; }
}
