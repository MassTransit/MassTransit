namespace MassTransit.KafkaIntegration.Transport
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;
    using Riders;
    using Transports;


    public class KafkaRider :
        IKafkaRider
    {
        readonly IReceiveEndpointCollection _endpoints;
        readonly Uri _hostAddress;
        readonly IKafkaHostConfiguration _hostConfiguration;
        readonly Dictionary<Uri, IKafkaProducerFactory> _producerFactories;

        public KafkaRider(IKafkaHostConfiguration hostConfiguration, Uri hostAddress, IReceiveEndpointCollection endpoints,
            IEnumerable<IKafkaProducerFactory> producerFactories)
        {
            _hostConfiguration = hostConfiguration;
            _hostAddress = hostAddress;
            _endpoints = endpoints;
            _producerFactories = producerFactories.ToDictionary(x => x.TopicAddress);
        }

        public ITopicProducer<TKey, TValue> GetProducer<TKey, TValue>(Uri address, ConsumeContext consumeContext)
            where TValue : class
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));

            var topicAddress = new KafkaTopicAddress(_hostAddress, address);

            IKafkaProducerFactory<TKey, TValue> factory = GetProducerFactory<TKey, TValue>(topicAddress);

            return factory.CreateProducer(consumeContext);
        }

        public RiderHandle Start(CancellationToken cancellationToken = default)
        {
            HostReceiveEndpointHandle[] endpointsHandle = _endpoints.StartEndpoints(cancellationToken);
            IAgent agent = new RiderAgent(_hostConfiguration.ClientContextSupervisor, _endpoints);
            return new Handle(endpointsHandle, agent);
        }

        IKafkaProducerFactory<TKey, TValue> GetProducerFactory<TKey, TValue>(Uri topicAddress)
            where TValue : class
        {
            if (!_producerFactories.TryGetValue(topicAddress, out var factory))
                throw new ConfigurationException($"Producer for topic: {topicAddress} is not configured.");

            if (factory is IKafkaProducerFactory<TKey, TValue> producerFactory)
                return producerFactory;

            throw new ConfigurationException($"Producer for topic: {topicAddress} is not configured for ${typeof(Message<TKey, TValue>).Name} message");
        }


        class RiderAgent :
            Agent
        {
            readonly IReceiveEndpointCollection _endpoints;
            readonly IClientContextSupervisor _supervisor;

            public RiderAgent(IClientContextSupervisor supervisor, IReceiveEndpointCollection endpoints)
            {
                _supervisor = supervisor;
                _endpoints = endpoints;

                SetReady(_supervisor.Ready);
                SetCompleted(_supervisor.Completed);
            }

            protected override async Task StopAgent(StopContext context)
            {
                await _endpoints.Stop(context.CancellationToken).ConfigureAwait(false);
                await _supervisor.Stop(context).ConfigureAwait(false);
                await base.StopAgent(context).ConfigureAwait(false);
            }
        }


        class Handle :
            RiderHandle
        {
            readonly IAgent _agent;
            readonly HostReceiveEndpointHandle[] _endpoints;

            public Handle(HostReceiveEndpointHandle[] endpoints, IAgent agent)
            {
                _endpoints = endpoints;
                _agent = agent;
            }

            public Task Ready => ReadyOrNot(_endpoints.Select(x => x.Ready));

            public Task StopAsync(CancellationToken cancellationToken)
            {
                return _agent.Stop("Kafka stopped", cancellationToken);
            }

            async Task ReadyOrNot(IEnumerable<Task<ReceiveEndpointReady>> endpoints)
            {
                Task<ReceiveEndpointReady>[] readyTasks = endpoints as Task<ReceiveEndpointReady>[] ?? endpoints.ToArray();
                foreach (Task<ReceiveEndpointReady> ready in readyTasks)
                    await ready.ConfigureAwait(false);

                await _agent.Ready.ConfigureAwait(false);

                await Task.WhenAll(readyTasks).ConfigureAwait(false);
            }
        }
    }
}
