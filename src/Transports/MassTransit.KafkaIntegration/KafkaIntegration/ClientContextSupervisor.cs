namespace MassTransit.KafkaIntegration
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Configuration;
    using Confluent.Kafka;
    using Transports;


    public class ClientContextSupervisor :
        TransportPipeContextSupervisor<ClientContext>,
        IClientContextSupervisor
    {
        readonly ConcurrentDictionary<Uri, IKafkaProducerFactory> _factories;
        readonly IReadOnlyDictionary<string, IKafkaProducerSpecification> _producers;

        public ClientContextSupervisor(ClientConfig clientConfig, IEnumerable<IKafkaProducerSpecification> producers)
            : base(new ClientContextFactory(clientConfig))
        {
            _producers = producers.ToDictionary(x => x.TopicName);
            _factories = new ConcurrentDictionary<Uri, IKafkaProducerFactory>();
        }

        public ITopicProducer<TKey, TValue> CreateProducer<TKey, TValue>(IBusInstance busInstance, Uri address)
            where TValue : class
        {
            if (busInstance == null)
                throw new ArgumentNullException(nameof(busInstance));

            if (address == null)
                throw new ArgumentNullException(nameof(address));

            var topicAddress = NormalizeAddress(busInstance.HostConfiguration.HostAddress, address);

            IKafkaProducerFactory<TKey, TValue> factory = GetFactory<TKey, TValue>(topicAddress, busInstance);
            return factory.CreateProducer();
        }

        static KafkaTopicAddress NormalizeAddress(Uri hostAddress, Uri address)
        {
            return new KafkaTopicAddress(hostAddress, address);
        }

        IKafkaProducerFactory<TKey, TValue> GetFactory<TKey, TValue>(KafkaTopicAddress address, IBusInstance busInstance)
            where TValue : class
        {
            if (!_producers.TryGetValue(address.Topic, out var specification))
                throw new ConfigurationException($"Producer for topic: {address} is not configured.");

            var factory = _factories.GetOrAdd(address, _ => specification.CreateProducerFactory(busInstance));

            if (factory is IKafkaProducerFactory<TKey, TValue> f)
                return f;

            throw new ConfigurationException($"Producer for topic: {address} is not configured for ${typeof(Message<TKey, TValue>).Name} message");
        }
    }
}
