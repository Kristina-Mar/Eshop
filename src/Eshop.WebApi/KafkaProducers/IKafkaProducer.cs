using Confluent.Kafka;

namespace Eshop.WebApi.KafkaProducers
{
    public interface IKafkaProducer
    {
        public Task<DeliveryResult<int, string>> ProduceAsync(string topic, Message<int, string> message);
    }
}