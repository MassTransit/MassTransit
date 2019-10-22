namespace MassTransit.AzureServiceBusTransport.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;
    using GreenPipes.Builders;
    using GreenPipes.Configurators;
    using MassTransit.Configuration;
    using MassTransit.Pipeline.Filters;
    using Pipeline;
    using Settings;
    using Topology.Configuration;
    using Transport;
    using Transports;
    using Util;


    public abstract class ServiceBusEntityReceiveEndpointConfiguration :
        ReceiveEndpointConfiguration
    {
        readonly IEndpointEntityConfigurator _configurator;
        readonly IServiceBusHostConfiguration _hostConfiguration;
        readonly BaseClientSettings _settings;
        protected readonly IBuildPipeConfigurator<ClientContext> ClientPipeConfigurator;
        protected readonly IBuildPipeConfigurator<MessagingFactoryContext> MessagingFactoryPipeConfigurator;
        protected readonly IBuildPipeConfigurator<NamespaceContext> NamespacePipeConfigurator;

        protected ServiceBusEntityReceiveEndpointConfiguration(IServiceBusHostConfiguration hostConfiguration, BaseClientSettings settings,
            IServiceBusEndpointConfiguration endpointConfiguration)
            : base(endpointConfiguration)
        {
            _hostConfiguration = hostConfiguration;
            _settings = settings;
            _configurator = settings.Configurator;

            ClientPipeConfigurator = new PipeConfigurator<ClientContext>();
            NamespacePipeConfigurator = new PipeConfigurator<NamespaceContext>();
            MessagingFactoryPipeConfigurator = new PipeConfigurator<MessagingFactoryContext>();
        }

        public int MaxConcurrentCalls
        {
            set
            {
                _settings.MaxConcurrentCalls = value;
                if (_settings.MaxConcurrentCalls > _settings.PrefetchCount)
                    _settings.PrefetchCount = _settings.MaxConcurrentCalls;
            }
        }

        public int PrefetchCount
        {
            set => _settings.PrefetchCount = value;
        }

        public TimeSpan AutoDeleteOnIdle
        {
            set
            {
                _configurator.AutoDeleteOnIdle = value;

                Changed(nameof(AutoDeleteOnIdle));
            }
        }

        public TimeSpan DefaultMessageTimeToLive
        {
            set => _configurator.DefaultMessageTimeToLive = value;
        }

        public bool EnableBatchedOperations
        {
            set => _configurator.EnableBatchedOperations = value;
        }

        public bool EnableDeadLetteringOnMessageExpiration
        {
            set => _configurator.EnableDeadLetteringOnMessageExpiration = value;
        }

        public string ForwardDeadLetteredMessagesTo
        {
            set => _configurator.ForwardDeadLetteredMessagesTo = value;
        }

        public TimeSpan LockDuration
        {
            set => _configurator.LockDuration = value;
        }

        public int MaxDeliveryCount
        {
            set => _configurator.MaxDeliveryCount = value;
        }

        public bool RequiresSession
        {
            set => _configurator.RequiresSession = value;
        }

        public string UserMetadata
        {
            set => _configurator.UserMetadata = value;
        }

        public virtual void SelectBasicTier()
        {
            _settings.SelectBasicTier();
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            return ClientPipeConfigurator.Validate()
                .Concat(NamespacePipeConfigurator.Validate())
                .Concat(MessagingFactoryPipeConfigurator.Validate())
                .Concat(ValidateSettings())
                .Concat(base.Validate());
        }

        IEnumerable<ValidationResult> ValidateSettings()
        {
            if (_settings.PrefetchCount < 0)
                yield return this.Failure("PrefetchCount", "must be >= 0");

            if (_settings.MaxConcurrentCalls <= 0)
                yield return this.Failure("MaxConcurrentCalls", "must be > 0");
        }

        protected void CreateReceiveEndpoint(IServiceBusHostControl host, ServiceBusReceiveEndpointContext receiveEndpointContext)
        {
            var transportObserver = receiveEndpointContext.TransportObservers;

            IAgent consumerAgent;
            if (_hostConfiguration.DeployTopologyOnly)
            {
                var transportReadyFilter = new TransportReadyFilter<ClientContext>(receiveEndpointContext);
                ClientPipeConfigurator.UseFilter(transportReadyFilter);

                consumerAgent = transportReadyFilter;
            }
            else
            {
                var messageReceiver = new BrokeredMessageReceiver(InputAddress, receiveEndpointContext);

                var errorTransport = CreateErrorTransport(host);
                var deadLetterTransport = CreateDeadLetterTransport(host);

                receiveEndpointContext.GetOrAddPayload(() => deadLetterTransport);
                receiveEndpointContext.GetOrAddPayload(() => errorTransport);

                var receiverFilter = _settings.RequiresSession
                    ? new MessageSessionReceiverFilter(messageReceiver, transportObserver)
                    : new MessageReceiverFilter(messageReceiver, transportObserver);

                ClientPipeConfigurator.UseFilter(receiverFilter);

                consumerAgent = receiverFilter;
            }

            IPipe<ClientContext> clientPipe = ClientPipeConfigurator.Build();

            var clientCache = CreateClientCache(host.MessagingFactoryContextSupervisor, host.NamespaceContextSupervisor);

            var transport = new ReceiveTransport(host, _settings, clientCache, clientPipe, receiveEndpointContext);

            transport.Add(consumerAgent);

            var receiveEndpoint = new ReceiveEndpoint(transport, receiveEndpointContext);

            var queueName = _settings.Path ?? NewId.Next().ToString(FormatUtil.Formatter);

            host.AddReceiveEndpoint(queueName, receiveEndpoint);

            ReceiveEndpoint = receiveEndpoint;
        }

        protected abstract IErrorTransport CreateErrorTransport(IServiceBusHostControl host);
        protected abstract IDeadLetterTransport CreateDeadLetterTransport(IServiceBusHostControl host);

        protected abstract IClientContextSupervisor CreateClientCache(IMessagingFactoryContextSupervisor messagingFactoryContextSupervisor,
            INamespaceContextSupervisor namespaceContextSupervisor);

        protected virtual IPipeContextFactory<SendEndpointContext> CreateSendEndpointContextFactory(IServiceBusHost host, SendSettings settings,
            IPipe<NamespaceContext> namespacePipe)
        {
            return new QueueSendEndpointContextFactory(host.MessagingFactoryContextSupervisor, host.NamespaceContextSupervisor,
                Pipe.Empty<MessagingFactoryContext>(), namespacePipe, settings);
        }

        protected ISendEndpointContextSupervisor CreateSendEndpointContextSupervisor(IServiceBusHostControl host, SendSettings settings)
        {
            var brokerTopology = settings.GetBrokerTopology();

            IPipe<NamespaceContext> namespacePipe = new ConfigureTopologyFilter<SendSettings>(settings, brokerTopology, false, host.Stopping).ToPipe();

            IPipeContextFactory<SendEndpointContext> factory = CreateSendEndpointContextFactory(host, settings, namespacePipe);

            var supervisor = new SendEndpointContextSupervisor(factory);
            host.Add(supervisor);

            return supervisor;
        }
    }
}
