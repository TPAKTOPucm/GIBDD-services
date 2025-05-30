using Microsoft.EntityFrameworkCore;

namespace Core.Aggregates.Common;

[Owned]
public record Address(uint ZipCode, string City, string CityType, string Street, uint? Home, uint? Office);
[Owned]
public record FullName(string FirstName, string LastName, string MiddleName);
