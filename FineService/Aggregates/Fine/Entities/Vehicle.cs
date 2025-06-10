using FineService.Aggregates.Common;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Text.Json.Serialization;

namespace FineService.Aggregates.Fine.Entities;

public class Vehicle : Entity<Guid>
{
    ICollection<Fine> _fines;
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
    public Guid Id { get; init; }
    public LicensePlate? LicensePlate { get; private set; }
    public string Make { get; init; }
    public string Model { get; init; }
    [JsonIgnore]
    public ICollection<Fine> Fines { get => _lazyLoader.Load(this, ref _fines); }
}
