namespace MassTransit.KafkaIntegration.Transport
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Pipeline.Observables;
    using Riders;
    using Util;


    public class KafkaRider :
        BaseRider,
        IKafkaRider
    {
        readonly IKafkaReceiveEndpoint[] _endpoints;
        readonly Uri _hostAddress;
        readonly Dictionary<Uri, IKafkaProducerFactory> _producerFactories;

        public KafkaRider(Uri hostAddress, IEnumerable<IKafkaReceiveEndpoint> endpoints, IEnumerable<IKafkaProducerFactory> producerFactories,
            RiderObservable observers)
            : base("confluent.kafka", observers)
        {
            _hostAddress = hostAddress;
            _endpoints = endpoints?.ToArray() ?? Array.Empty<IKafkaReceiveEndpoint>();
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

        protected override Task StartRider(CancellationToken cancellationToken)
        {
            if (!_endpoints.Any())
                return TaskUtil.Completed;

            return Task.WhenAll(_endpoints.Select(endpoint => endpoint.Start(cancellationToken)));
        }

        protected override async Task StopRider(CancellationToken cancellationToken)
        {
            if (_endpoints.Any())
                await Task.WhenAll(_endpoints.Select(endpoint => endpoint.Stop())).ConfigureAwait(false);

            foreach (var factory in _producerFactories.Values)
                factory.Dispose();
        }
    }
}
