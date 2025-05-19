using Core.Aggregates.VehicleRegistration.Entities;
using Core.Data;
using Microsoft.EntityFrameworkCore;

namespace Core.Repository;

public class DriverRepository : IDriverRepository
{
    private readonly CarsContext _db;
    public DriverRepository(CarsContext db)
    {
        _db = db;
    }

    public Task AddAsync(Driver driver)
    {
        _db.Drivers.Add(driver);
        return _db.SaveChangesAsync();
    }

    public Task<Driver> GetByIdAsync(Guid id) =>
        _db.Drivers.FirstOrDefaultAsync(d => d.Id == id);
}
