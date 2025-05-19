using Core.Aggregates.Fine;
using Core.Aggregates.VehicleRegistration.Entities;
using Core.Data;
using Microsoft.EntityFrameworkCore;

namespace Core.Repository;

public class FineRepository : IFineRepository
{
    private readonly CarsContext _db;
    public FineRepository(CarsContext db)
    {
        _db = db;
    }
    public Task Add(Fine fine)
    {
        _db.Add(fine);
        return _db.SaveChangesAsync();
    }

    public Task<List<Fine>> GetByVehiclePlate(LicensePlate plate) =>
        _db.Fines.Where(f => f.Vehicle.LicensePlate == plate).ToListAsync();

    public Task<Fine> GetById(Guid id) =>
        _db.Fines.Include(f => f.Receipt).FirstOrDefaultAsync(f => f.Id == id);
}
