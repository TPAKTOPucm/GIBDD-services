using Core.Aggregates.Common;
using Core.Aggregates.VehicleRegistration.Entities;
using Core.Aggregates.VehicleRegistration.Events;

namespace Core.Aggregates.VehicleRegistration;

public class VehicleRegistration : Aggregate<Guid>
{
    public Guid VehicleId { get; init; }
    public Vehicle Vehicle { get; init; }
    public Guid DriverId { get; init; }
    public Driver Driver { get; init; }
    public DateTime RegistrationDate { get; init; }
    public DateTime? DeregistrationDate { get; private set; }

    protected VehicleRegistration() { }
    public VehicleRegistration(Vehicle vehicle, Driver driver, LicensePlate licensePlate, IEnumerable<Vehicle> vehicles)
    {
        Vehicle = vehicle;
        Driver = driver;
        if (!vehicle.Register(licensePlate, vehicles)) throw new ArgumentException("Can not register this vehicle");
        RegistrationDate = DateTime.UtcNow;
        AddDomainEvent(new VehicleRegisteredEvent(vehicle.Id,
            vehicle.Make,
            vehicle.Model,
            licensePlate,
            driver.Id,
            driver.FullName,
            RegistrationDate));
    }

    public bool Deregister()
    {
        if (Vehicle.Deregister())
        {
            DeregistrationDate = DateTime.UtcNow;
            AddDomainEvent(new VehicleDeregisteredEvent(Vehicle.Id,
                Vehicle.Make,
                Vehicle.Model,
                Vehicle.LicensePlate,
                Driver.Id,
                Driver.FullName,
                RegistrationDate,
                (DateTime)DeregistrationDate));
            return true;
        }
        return false;
    }
}
