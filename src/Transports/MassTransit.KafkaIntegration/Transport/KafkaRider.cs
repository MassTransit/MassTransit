namespace MassTransit.KafkaIntegration.Transport
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Confluent.Kafka;
    using Riders;


    public class KafkaRider :
        BaseRider,
        IKafkaRider
    {
        readonly IDictionary<string, IReceiveEndpointControl> _endpoints;
        readonly Uri _hostAddress;
        readonly Dictionary<Uri, IKafkaProducerFactory> _producerFactories;

        public KafkaRider(Uri hostAddress, IDictionary<string, IReceiveEndpointControl> endpoints, IEnumerable<IKafkaProducerFactory> producerFactories)
            : base("confluent.kafka")
        {
            _hostAddress = hostAddress;
            _endpoints = endpoints ?? new Dictionary<string, IReceiveEndpointControl>();
            _producerFactories = producerFactories.ToDictionary(x => x.TopicAddress);
        }

        public ITopicProducer<TKey, TValue> GetProducer<TKey, TValue>(Uri address, ConsumeContext consumeContext)
            where TValue : class
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));

            var topicAddress = new KafkaTopicAddress(_hostAddress, address);

            KafkaProducerFactory<TKey, TValue> factory = GetProducerFactory<TKey, TValue>(topicAddress);

            return factory.CreateProducer(consumeContext);
        }

        KafkaProducerFactory<TKey, TValue> GetProducerFactory<TKey, TValue>(Uri topicAddress)
            where TValue : class
        {
            if (!_producerFactories.TryGetValue(topicAddress, out var factory))
                throw new ConfigurationException($"Producer for topic: {topicAddress} is not configured.");

            if (factory is KafkaProducerFactory<TKey, TValue> producerFactory)
                return producerFactory;

            throw new ConfigurationException($"Producer for topic: {topicAddress} is not configured for ${typeof(Message<TKey, TValue>).Name} message");
        }

        protected override void AddReceiveEndpoint(IHost host)
        {
            foreach (KeyValuePair<string, IReceiveEndpointControl> endpoint in _endpoints)
                host.AddReceiveEndpoint(endpoint.Key, endpoint.Value);
        }
    }
}
