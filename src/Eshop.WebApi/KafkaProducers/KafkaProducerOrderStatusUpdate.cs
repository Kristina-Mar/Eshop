using Confluent.Kafka;

namespace Eshop.WebApi.KafkaProducers
{
    public class KafkaProducerOrderStatusUpdate : IKafkaProducer
    {
        private readonly IProducer<int, string> _producer;

        public KafkaProducerOrderStatusUpdate()
        {
            var config = new ProducerConfig
            {
                BootstrapServers = Environment.GetEnvironmentVariable("BOOTSTRAP_SERVER") ?? "localhost:29092,localhost:39092,localhost:49092", // identifies one or more Kafka brokers, it will be used to establish the original connection to the Kafka cluster
                ClientId = "OrderStatusUpdate", // used to identify the producer
                Acks = Acks.All // defines whether the producer will wait for a response from the broker
                //Some other possible settings below, not necessary for this app
                //SecurityProtocol = SecurityProtocol.SaslSsl,
                //MessageTimeoutMs = 30000,
                //BatchNumMessages = 10000,
                //LingerMs = 5,
                //CompressionType = CompressionType.Gzip
            };
            _producer = new ProducerBuilder<int, string>(config).Build();
        }
        
        public async Task<DeliveryResult<int, string>> ProduceAsync(string topic, Message<int, string> message)
        {
            return await _producer.ProduceAsync(topic, message);
        }
    }
}