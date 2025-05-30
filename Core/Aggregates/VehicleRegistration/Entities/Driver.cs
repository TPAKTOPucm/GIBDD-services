using Core.Aggregates.Common;

namespace Core.Aggregates.VehicleRegistration.Entities;

public class Driver : Entity<Guid>
{
    protected Driver() { }
    public Guid Id { get; init; }
    public FullName FullName { get; init; }
    public DateTime BirthDate { get; init; }
    public Address Address { get; private set; }

    public Driver(FullName fullName, DateTime birthDate, Address address)
    {
        Id = Guid.NewGuid();
        FullName = fullName;
        BirthDate = birthDate;
        Address = address;
    }
}
