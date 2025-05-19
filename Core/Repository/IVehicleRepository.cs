using Core.Aggregates.VehicleRegistration;
using Core.Aggregates.VehicleRegistration.Entities;

namespace Core.Repository;

public interface IVehicleRepository
{
    Task<Vehicle> GetById(Guid vehicleId);
    Task<Vehicle> GetByPlate(LicensePlate licensePlate);
    Task<VehicleRegistration> GetRegistrationById(Guid id);
    Task Add(Vehicle vehicle);
    Task Add(VehicleRegistration vehicleRegistration);
    Task<List<Vehicle>> GetByDriverId(Guid driverId);
}
