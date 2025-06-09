using FineService.Aggregates.Fine;
using FineService.Aggregates.Fine.Entities;

namespace FineService.Repository;

public interface IFineRepository
{
    Task<Fine> GetById(Guid id);
    Task Add(Fine fine);
    Task Update(Fine fine);
    Task<List<Fine>> GetByLicensePlate(LicensePlate plate);
}
