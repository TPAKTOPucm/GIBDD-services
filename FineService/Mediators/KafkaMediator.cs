using Confluent.Kafka;
using System.Text.Json;
using System.Text.Json.Serialization;

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
		using var kafkaProducer = new ProducerBuilder<string, string>(_config).Build();
		Console.WriteLine("\n\n\n\n\n");
		return kafkaProducer.ProduceAsync("fines", new Message<string, string>
		{
			Key = notification.GetHashCode().ToString(),
			Value = JsonSerializer.Serialize(notification, new JsonSerializerOptions { Converters = {new JsonStringEnumConverter()}})
		});

		//Console.WriteLine($"Событие успешно опубликовано в топик {result.Topic}, партиция {result.Partition}, смещение {result.Offset}");
	}
}
