using MediatR;
using Core.DTOs;
using Microsoft.AspNetCore.Mvc;
using Core.Repository;
using Core.Data;
using Microsoft.EntityFrameworkCore;
using Core.Aggregates.VehicleRegistration.Entities;
using Core.Aggregates.VehicleRegistration;
using Core.Aggregates.Fine;
using Core.Factory;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());
builder.Services.AddDbContext<CarsContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres"))
);
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<IDriverRepository, DriverRepository>();
builder.Services.AddScoped<IFineRepository, FineRepository>();

var app = builder.Build();

app.MapPost("/api/drivers", async (IDriverRepository repository, [FromBody] CreateDriverRequest request) =>
{
    var driver = new Driver(request.FullName, request.BirthDate, request.Address);
    await repository.AddAsync(driver);
    return Results.Created($"/api/drivers/{driver.Id}", driver.Id);
});

app.MapPost("/api/vehicles", async (IVehicleRepository vehicleRepository, IDriverRepository driverRepository, [FromBody] RegisterVehicleRequest request) =>
{
    Vehicle vehicle;
    Driver driver;
    if(request.Vehicle.Id is not null)
        vehicle = await vehicleRepository.GetById((Guid)request.Vehicle.Id);
    else
        vehicle = new Vehicle(request.Vehicle.Make, request.Vehicle.Model);

    if (request.Driver.Id is not null)
        driver = await driverRepository.GetByIdAsync((Guid)request.Driver.Id);
    else
        driver = new Driver(request.Driver.FullName, request.Driver.BirthDate, request.Driver.Address);

    var vehicles = new Vehicle[] { await vehicleRepository.GetByPlate(request.Vehicle.LicensePlate) };
    VehicleRegistration registration = new VehicleRegistration(vehicle, driver, request.Vehicle.LicensePlate, vehicles);

    await vehicleRepository.Add(registration);
    return Results.Created($"/api/vehicles/{vehicle.Id}", vehicle.Id);
});

app.MapDelete("/api/vehicles/{id}", async (IVehicleRepository vehicleRepository, Guid id) =>
{
    var registration = await vehicleRepository.GetRegistrationById(id);
    if (registration is null)
        return Results.NotFound();
    if (registration.Deregister())
        return Results.Ok();
    return Results.StatusCode(451);
});

app.MapPost("/api/fines/{id}/pay", async (IFineRepository fineRepository, Guid id, [FromBody] Guid paymentTransactionId) =>
{
    var fine = await fineRepository.GetById(id);
    if (fine.Pay(paymentTransactionId))
        return Results.Ok();
    return Results.BadRequest();
});

app.MapGet("/api/drivers/{driverId}/vehicles", async (IVehicleRepository vehicleRepository, Guid driverId) =>
    await vehicleRepository.GetByDriverId(driverId));

app.MapGet("/api/vehicles/fines", async (IFineRepository fineRepository, LicensePlate plate) =>
    await fineRepository.GetByVehiclePlate(plate));

app.MapPost("/api/fines", async (IVehicleRepository vehicleRepository, IFineRepository fineRepository, [FromBody] IssueFineRequest request) =>
{
    var vehicle = await vehicleRepository.GetByPlate(request.Plate);
    if (vehicle is null)
        return Results.NotFound();
    var receipt = await PaymentReceiptFactory.Create();
    var fine = new Fine(request.Reason, request.IssueDate, receipt, vehicle);
    await fineRepository.Add(fine);
    return Results.Ok();
});

app.MapPost("/api/vehicles/{id}/confiscate", async (IVehicleRepository vehicleRepository, Guid id, [FromBody] ConfiscateRequest request) =>
{
    var vehicle = await vehicleRepository.GetById(id);
    vehicle.Confiscate(request.Reason);
    vehicleRepository.Add(vehicle);
    return Results.Ok();
});

app.Run();