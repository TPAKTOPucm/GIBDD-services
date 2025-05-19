namespace Core.Aggregates.Common;

public record Address(uint ZipCode, string City, string CityType, string Street, uint? Home, uint? Office);

public record FullName(string FirstName, string LastName, string MiddleName);
