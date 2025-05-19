using Core.Aggregates.Common;

namespace Core.Aggregates.VehicleRegistration.Entities;

public class Driver : Entity<Guid>
{
    public Guid Id { get; }
    public FullName FullName { get; }
    public DateTime BirthDate { get; }
    public Address Address { get; }

    public Driver(FullName fullName, DateTime birthDate, Address address)
    {
        Id = Guid.NewGuid();
        FullName = fullName;
        BirthDate = birthDate;
        Address = address;
    }
}
