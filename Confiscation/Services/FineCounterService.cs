using Confluent.Kafka;
using ConfiscationService.Data;
using System.Text.Json;
using ConfiscationService.Aggregates.Fine.Events;
using Microsoft.EntityFrameworkCore;

namespace ConfiscationService.Services;

public class FineCounterService : BackgroundService
{
    private readonly ConfiscationContext _db;
    private readonly ConsumerConfig _consumerConfig;
    public FineCounterService(ConfiscationContext db, ConsumerConfig config)
    {
        _db = db;
        _consumerConfig = config;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		using (var consumer = new ConsumerBuilder<Ignore, string>(_consumerConfig).Build())
        {
            consumer.Subscribe("fines");
            while (!stoppingToken.IsCancellationRequested)
            {
                var fine = JsonSerializer.Deserialize<FineStatusChanged>(consumer.Consume(stoppingToken).Value);
                var vehicle = await _db.Vehicles.Where(v => v.LicensePlate.BaseNumber == fine.LicensePlate.BaseNumber && v.LicensePlate.Region == fine.LicensePlate.Region)
                        .FirstOrDefaultAsync();
                if (vehicle is null)
                {
                    vehicle = new()
                    {
                        LicensePlate = fine.LicensePlate
                    };
                    _db.Add(vehicle);
                }
                if (fine.FineStatus == FineStatus.Confirmed)
                    vehicle.UnpaidFineCount++;
                else if (fine.FineStatus == FineStatus.Paid)
                    vehicle.UnpaidFineCount--;
                await _db.SaveChangesAsync();
            }
        }
	}
}
