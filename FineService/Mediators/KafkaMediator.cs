using Confluent.Kafka;
using System.Text.Json;

namespace FineService.Mediators;

public class KafkaMediator : IMediator
{
	private readonly ProducerConfig _config;
    public KafkaMediator(ProducerConfig config)
    {
		_config = config;
    }

    public Task Publish(object notification)
	{
		using var kafkaProducer = new ProducerBuilder<Ignore, string>(_config).Build();
		return kafkaProducer.ProduceAsync("fines", new Message<Ignore, string>
		{
			Value = JsonSerializer.Serialize(notification)
		});
	}
}
