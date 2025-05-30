using Core.Aggregates.Fine;
using Core.Aggregates.VehicleRegistration.Entities;
using Core.Aggregates.VehicleRegistration;
using Core.Data;
using Microsoft.EntityFrameworkCore;

namespace Core.Repository;

public class VehicleRepository : IVehicleRepository
{
    private readonly CarsContext _db;
    public VehicleRepository(CarsContext db)
    {
        _db = db;
    }

    public Task<Vehicle?> GetById(Guid id) => _db.Vehicles.Where(v => v.Id == id).Include(v => v.Fines).FirstOrDefaultAsync();

    public Task Add(VehicleRegistration registration)
    {
        _db.Add(registration);
        return _db.SaveChangesAsync();
    }

    public Task<Vehicle> GetByPlate(LicensePlate licensePlate) =>
        _db.Vehicles.FirstOrDefaultAsync(v => v.LicensePlate.BaseNumber == licensePlate.BaseNumber && v.LicensePlate.Region == licensePlate.Region);

    public Task<VehicleRegistration> GetRegistrationById(Guid id) => _db.VehicleRegistrations.FirstOrDefaultAsync(v => v.Id == id);

    public Task Add(Vehicle vehicle)
    {
        _db.Add(vehicle);
        return _db.SaveChangesAsync();
    }

    public Task<List<Vehicle>> GetByDriverId(Guid driverId) =>
        _db.VehicleRegistrations.Where(v => v.DriverId == driverId).Select(v => v.Vehicle).ToListAsync();
}
