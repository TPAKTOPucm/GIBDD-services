using Core.Aggregates.Fine;
using Core.Aggregates.VehicleRegistration.Entities;

namespace Core.Repository;

public interface IFineRepository
{
    Task<Fine> GetById(Guid id);
    Task Add(Fine fine);
    Task<List<Fine>> GetByVehiclePlate(LicensePlate plate);
}
