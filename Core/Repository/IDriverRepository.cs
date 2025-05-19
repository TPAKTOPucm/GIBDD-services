using Core.Aggregates.VehicleRegistration.Entities;

namespace Core.Repository;

public interface IDriverRepository
{
    Task AddAsync(Driver driver);
    Task<Driver> GetByIdAsync(Guid id);
}
