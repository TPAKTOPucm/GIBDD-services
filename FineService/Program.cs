using FineService.Factory;
using FineService.Repository;
using Microsoft.AspNetCore.Mvc;
using FineService.Data;
using Microsoft.EntityFrameworkCore;
using FineService.Mediators;
using Confluent.Kafka;
using FineService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

PaymentReceiptFactory.PriceServiceUrl = builder.Configuration.GetConnectionString("PriceService");

var consumerConfig = new ConsumerConfig
{
	GroupId = "FineService",
	BootstrapServers = builder.Configuration.GetConnectionString("Kafka"),
	AutoOffsetReset = AutoOffsetReset.Earliest
};

builder.Services.AddSingleton(consumerConfig);

builder.Services.AddSingleton<IMediator>(new KafkaMediator(new ProducerConfig(consumerConfig)));

builder.Services.AddDbContext<FinesContext>(options =>
	options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")), ServiceLifetime.Scoped, ServiceLifetime.Singleton
);

builder.Services.AddScoped<IFineRepository, FineRepository>();

builder.Services.AddHostedService<RawFineService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}


app.MapGet("/api/{id}", async (Guid id, [FromServices] IFineRepository repository) =>
{
	var fine = await repository.GetById(id);
	return new { fine.Id, fine.Reason, fine.Vehicle, fine.IssueDate, fine.Status };
}).WithOpenApi();

app.MapDelete("/api/{id}", async (Guid id, [FromServices] IFineRepository repository) =>
{
	var fine = await repository.GetById(id);
	if (!fine.Reject())
		return Results.BadRequest();
	await repository.Update(fine);
	return Results.Ok();
}).WithOpenApi();

app.MapPost("/api/{id}", async (Guid id, [FromServices] IFineRepository repository) =>
{
	var fine = await repository.GetById(id);
	var receipt = await PaymentReceiptFactory.Create(new(fine.Reason, fine.IssueDate));
	if (!fine.Confirm(receipt))
		return Results.BadRequest();
	await repository.Update(fine);
	return Results.Ok();
}).WithOpenApi();

app.MapPost("/api/payment/{id}", async (Guid id, [FromBody] Guid transactionId, [FromServices] IFineRepository repository) =>
{
	var fine = await repository.GetById(id);
	if (!fine.Pay(transactionId))
		return Results.BadRequest();
	await repository.Update(fine);
	return Results.Ok();
}).WithOpenApi();

app.Run();
