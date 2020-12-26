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
    using MassTransit.Registration;
    using Riders;
    using Transports;
    using Util;


    public class KafkaRider :
        IKafkaRider
    {
        readonly IReceiveEndpointCollection _endpoints;
        readonly IKafkaHostConfiguration _hostConfiguration;
        readonly ITopicProducerProvider _producerProvider;
        readonly ITopicEndpointConnector _topicEndpointConnector;

        public KafkaRider(IKafkaHostConfiguration hostConfiguration, IReceiveEndpointCollection endpoints, ITopicProducerProvider producerProvider,
            ITopicEndpointConnector topicEndpointConnector)
        {
            _hostConfiguration = hostConfiguration;
            _endpoints = endpoints;
            _producerProvider = producerProvider;
            _topicEndpointConnector = topicEndpointConnector;
        }

        public ITopicProducer<TKey, TValue> GetProducer<TKey, TValue>(Uri address, ConsumeContext consumeContext)
            where TValue : class
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));

            var provider = consumeContext == null
                ? _producerProvider
                : new ConsumeContextTopicProducerProvider(_producerProvider, consumeContext);
            return provider.GetProducer<TKey, TValue>(address);
        }

        public HostReceiveEndpointHandle ConnectTopicEndpoint<TKey, TValue>(string topicName, string groupId,
            Action<IRiderRegistrationContext, IKafkaTopicReceiveEndpointConfigurator<TKey, TValue>> configure)
            where TValue : class
        {
            return _topicEndpointConnector.ConnectTopicEndpoint(topicName, groupId, configure);
        }

        public HostReceiveEndpointHandle ConnectTopicEndpoint<TKey, TValue>(string topicName, ConsumerConfig consumerConfig,
            Action<IRiderRegistrationContext, IKafkaTopicReceiveEndpointConfigurator<TKey, TValue>> configure)
            where TValue : class
        {
            return _topicEndpointConnector.ConnectTopicEndpoint(topicName, consumerConfig, configure);
        }

        public RiderHandle Start(CancellationToken cancellationToken = default)
        {
            HostReceiveEndpointHandle[] endpointsHandle = _endpoints.StartEndpoints(cancellationToken);
            var ready = endpointsHandle.Length == 0 ? TaskUtil.Completed : _hostConfiguration.ClientContextSupervisor.Ready;
            IAgent agent = new RiderAgent(_hostConfiguration.ClientContextSupervisor, _endpoints, ready);
            return new Handle(endpointsHandle, agent);
        }


        class RiderAgent :
            Agent
        {
            readonly IReceiveEndpointCollection _endpoints;
            readonly IClientContextSupervisor _supervisor;

            public RiderAgent(IClientContextSupervisor supervisor, IReceiveEndpointCollection endpoints, Task ready)
            {
                _supervisor = supervisor;
                _endpoints = endpoints;

                SetReady(ready);
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
