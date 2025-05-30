using Core.Aggregates.Common;
using Core.Aggregates.VehicleRegistration.Entities;

namespace Core.DTOs;

public record RegisterVehicleRequest(VehicleDto Vehicle, DriverDto Driver);
public class VehicleDto
{
    public Guid? Id { get; set; }
    public LicensePlate LicensePlate { get; set; }
    public string Make { get; set; }
    public string Model { get; set; }
}
public class DriverDto
{
    public Guid? Id { get; set; }
    public FullName FullName { get; set; }
    public DateTime BirthDate { get; set; }
    public Address Address { get; set; }
}
public record IssueFineRequest(LicensePlate Plate, string Reason, DateTime IssueDate);
public record ConfiscateRequest(string Reason);
public record CreateDriverRequest(FullName FullName, DateTime BirthDate, Address Address);

public record PaymentReceiptDto(Guid Id, decimal Price, ulong BankCode, ulong AccountCode);
public record PaymentReceiptFactoryDto(string Reason, DateTime IssueDate);