namespace MassTransit.KafkaIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using MassTransit.Middleware;
    using Transports;


    public class KafkaRider :
        IKafkaRider
    {
        readonly IBusInstance _busInstance;
        readonly IReceiveEndpointCollection _endpoints;
        readonly IKafkaHostConfiguration _hostConfiguration;
        readonly IRiderRegistrationContext _registrationContext;
        Lazy<ITopicProducerProvider> _producerProvider;

        public KafkaRider(IKafkaHostConfiguration hostConfiguration, IBusInstance busInstance, IReceiveEndpointCollection endpoints,
            IRiderRegistrationContext registrationContext)
        {
            _hostConfiguration = hostConfiguration;
            _busInstance = busInstance;
            _endpoints = endpoints;
            _registrationContext = registrationContext;

            Reset();
        }

        public ITopicProducer<TKey, TValue> GetProducer<TKey, TValue>(Uri address, ConsumeContext consumeContext)
            where TValue : class
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));

            var provider = consumeContext == null
                ? _producerProvider.Value
                : new ConsumeContextTopicProducerProvider(_producerProvider.Value, consumeContext);

            return provider.GetProducer<TKey, TValue>(address);
        }

        public HostReceiveEndpointHandle ConnectTopicEndpoint<TKey, TValue>(string topicName, string groupId,
            Action<IRiderRegistrationContext, IKafkaTopicReceiveEndpointConfigurator<TKey, TValue>> configure)
            where TValue : class
        {
            var specification = _hostConfiguration.CreateSpecification<TKey, TValue>(topicName, groupId, configurator =>
            {
                configure?.Invoke(_registrationContext, configurator);
            });

            _endpoints.Add(specification.EndpointName, specification.CreateReceiveEndpoint(_busInstance));

            return _endpoints.Start(specification.EndpointName);
        }

        public HostReceiveEndpointHandle ConnectTopicEndpoint<TKey, TValue>(string topicName, ConsumerConfig consumerConfig,
            Action<IRiderRegistrationContext, IKafkaTopicReceiveEndpointConfigurator<TKey, TValue>> configure)
            where TValue : class
        {
            var specification = _hostConfiguration.CreateSpecification<TKey, TValue>(topicName, consumerConfig, configurator =>
            {
                configure?.Invoke(_registrationContext, configurator);
            });

            _endpoints.Add(specification.EndpointName, specification.CreateReceiveEndpoint(_busInstance));

            return _endpoints.Start(specification.EndpointName);
        }

        public RiderHandle Start(CancellationToken cancellationToken = default)
        {
            HostReceiveEndpointHandle[] endpointsHandle = _endpoints.StartEndpoints(cancellationToken);

            var ready = endpointsHandle.Length == 0 ? Task.CompletedTask : _hostConfiguration.ClientContextSupervisor.Ready;

            var agent = new RiderAgent(_hostConfiguration.ClientContextSupervisor, _endpoints, ready, Reset);

            return new Handle(endpointsHandle, agent);
        }

        public IEnumerable<EndpointHealthResult> CheckEndpointHealth()
        {
            return _endpoints.CheckEndpointHealth();
        }

        void Reset()
        {
            _producerProvider = new Lazy<ITopicProducerProvider>(() => new TopicProducerProvider(_busInstance, _hostConfiguration));
        }


        class RiderAgent :
            Agent
        {
            readonly IReceiveEndpointCollection _endpoints;
            readonly Action _onStop;
            readonly IClientContextSupervisor _supervisor;

            public RiderAgent(IClientContextSupervisor supervisor, IReceiveEndpointCollection endpoints, Task ready, Action onStop)
            {
                _supervisor = supervisor;
                _endpoints = endpoints;
                _onStop = onStop;

                SetReady(ready);
                SetCompleted(_supervisor.Completed);
            }

            protected override async Task StopAgent(StopContext context)
            {
                await _endpoints.Stop(context.CancellationToken).ConfigureAwait(false);
                await _supervisor.Stop(context).ConfigureAwait(false);
                await base.StopAgent(context).ConfigureAwait(false);
                _onStop();
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
