using Core.Aggregates.Common;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Core.Aggregates.VehicleRegistration.Entities;

public class Vehicle : Entity<Guid>
{
    ICollection<Fine.Fine> _fines;
    ILazyLoader _lazyLoader;
    protected Vehicle(ILazyLoader loader)
    {
        _lazyLoader = loader;
    }
    public Vehicle(string make, string model)
    {
        Make = make;
        Model = model;
    }
    public Guid Id { get; }
    public LicensePlate? LicensePlate { get; private set; }
    public string Make { get; }
    public string Model { get; }
    public bool IsConfiscated { get => ConfiscationReason is not null; }
    public string? ConfiscationReason { get; private set; }
    public ICollection<Fine.Fine> Fines { get => _lazyLoader.Load(this, ref _fines); }

    private bool IsDeregistrable { get => Fines.All(f => f.IsPaid); }
    public bool Deregister()
    {
        if (IsDeregistrable)
        {
            LicensePlate = null;
            return true;
        }
        return false;
    }

    public bool Register(LicensePlate licensePlate, IEnumerable<Vehicle> vehicles)
    {
        if (LicensePlate is not null || vehicles.Any(v => v.LicensePlate == licensePlate))
            return false;
        LicensePlate = licensePlate;
        return true;
    }

    public bool Confiscate(string reason)
    {
        ConfiscationReason = reason;
        return true;
    }

    public bool ReturnCarToOwner()
    {
        ConfiscationReason = null;
        return true;
    }
}
