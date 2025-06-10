using Confluent.Kafka;
using FineService.Aggregates.Fine;
using FineService.Data;
using FineService.DTOs;
using FineService.Mediators;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace FineService.Services;

public class RawFineService : BackgroundService
{
	private readonly ILogger<RawFineService> _logger;
	private readonly FinesContext _db;
	private readonly ConsumerConfig _consumerConfig;
	public RawFineService(ILogger<RawFineService> logger, DbContextOptions dbContextOptions, IMediator mediator, ConsumerConfig config)
	{
		_db = new FinesContext(dbContextOptions, mediator);
		_consumerConfig = config;
		_logger = logger;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		using (var consumer = new ConsumerBuilder<Ignore, string>(_consumerConfig).Build())
		{
			consumer.Subscribe("violations");
			while (!stoppingToken.IsCancellationRequested)
			{
				var text = consumer.Consume(stoppingToken).Value;
				var dto = JsonSerializer.Deserialize<FineDto>(text);
				var fine = new Fine(dto.Id, dto.Reason, dto.IssueDate.ToUniversalTime(), dto.Vehicle);
				_db.Add(fine);
				_logger.LogInformation($"Reserved fine {dto.Id}");
				await _db.SaveChangesAsync();
			}
		}
	}
}
