using ConfiscationService.Data;
using ConfiscationService.DTOs;
using ConfiscationService.Events;
using ConfiscationService.Models;
using ConfiscationService.Services;
using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

var FINE_COUNT = 5;

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var consumerConfig = new ConsumerConfig {
	GroupId = "ConfiscationService",
	BootstrapServers = builder.Configuration.GetConnectionString("Kafka"),
	AutoOffsetReset = AutoOffsetReset.Earliest
};
var producerConfig = new ProducerConfig { BootstrapServers = consumerConfig.BootstrapServers };

builder.Services.AddSingleton(consumerConfig);

builder.Services.AddDbContext<ConfiscationContext>(options =>
	options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")), ServiceLifetime.Scoped, ServiceLifetime.Singleton
);

builder.Services.AddHostedService<FineCounterService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/api/confiscate", async ([FromBody] ConfiscationOrderDto dto, [FromServices] ConfiscationContext db) =>
{
	var confiscationOrder = new ConfiscationOrder
	{
		LicensePlate = dto.LicensePlate,
		Reason = dto.ConfiscationReason,
		Comment = dto.Comment,
		ImpoundYardAddress = dto.ImpoundYardAddress
	};
	if (confiscationOrder.Reason == ConfiscationReason.UnpaidFines)
	{
		var finesNumber = await db.Vehicles
			.Where(v => v.LicensePlate != null && v.LicensePlate.BaseNumber == dto.LicensePlate.BaseNumber &&
				v.LicensePlate.Region == dto.LicensePlate.Region).Select(v => v.UnpaidFineCount)
			.FirstOrDefaultAsync();
		if (finesNumber < FINE_COUNT)
			return Results.BadRequest("Not enough fines to confiscate this vehicle");
	}

	await Publish(new ConfiscationEvent(confiscationOrder, DateTime.UtcNow));

	db.Add(confiscationOrder);
	await db.SaveChangesAsync();
	return Results.Ok();
}).WithOpenApi();

app.MapDelete("/api/confiscate", async ([FromBody] LicensePlate licensePlate, [FromServices] ConfiscationContext db) =>
{
	var order = await db.ConfiscationOrders.Where(co => !co.IsReturned && co.LicensePlate != null && co.LicensePlate.BaseNumber == licensePlate.BaseNumber &&
	co.LicensePlate.Region == licensePlate.Region).FirstOrDefaultAsync();
	if (order is null)
		return Results.NotFound();

	order.IsReturned = true;

	await Publish(new ConfiscationEvent(order, DateTime.UtcNow));

	await db.SaveChangesAsync();
	return Results.Ok();
}).WithOpenApi();

app.Run();

Task Publish(ConfiscationEvent @event)
{
	using var kafkaProducer = new ProducerBuilder<Ignore, string>(producerConfig).Build();
	return kafkaProducer.ProduceAsync("confiscation", new Message<Ignore, string>
	{
		Value = JsonSerializer.Serialize(@event)
	});
}