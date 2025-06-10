using FineService.Aggregates.Fine;
using FineService.Aggregates.Fine.Entities;
using FineService.Data;
using Microsoft.EntityFrameworkCore;

namespace FineService.Repository;

public class FineRepository : IFineRepository
{
    private readonly FinesContext _db;
    public FineRepository(FinesContext db)
    {
        _db = db;
    }
    public Task Add(Fine fine)
    {
        _db.Add(fine);
        return _db.SaveChangesAsync();
    }

    public Task Update(Fine fine)
    {
        _db.Add(fine.Receipt);
        _db.Update(fine);
        return _db.SaveChangesAsync();
    }

    public Task<List<Fine>> GetByLicensePlate(LicensePlate plate) =>
        _db.Fines.Where(f => f.Vehicle.LicensePlate.Region == plate.Region && f.Vehicle.LicensePlate.BaseNumber == plate.BaseNumber).ToListAsync();

    public Task<Fine> GetById(Guid id) =>
        _db.Fines.Include(f => f.Receipt).FirstOrDefaultAsync(f => f.Id == id);
}
