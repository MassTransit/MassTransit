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


    public abstract class ServiceBusEntityReceiveEndpointConfiguration :
        ReceiveEndpointConfiguration
    {
        readonly IEndpointEntityConfigurator _configurator;
        readonly IServiceBusHostConfiguration _hostConfiguration;
        readonly BaseClientSettings _settings;
        protected readonly IBuildPipeConfigurator<ClientContext> ClientPipeConfigurator;
        protected readonly IBuildPipeConfigurator<MessagingFactoryContext> MessagingFactoryPipeConfigurator;
        protected readonly IBuildPipeConfigurator<NamespaceContext> NamespacePipeConfigurator;

        protected ServiceBusEntityReceiveEndpointConfiguration(IServiceBusHostConfiguration hostConfiguration, IServiceBusEndpointConfiguration configuration,
            BaseClientSettings settings)
            : base(hostConfiguration, configuration)
        {
            _hostConfiguration = hostConfiguration;
            _settings = settings;
            _configurator = settings.Configurator;

            ClientPipeConfigurator = new PipeConfigurator<ClientContext>();
            NamespacePipeConfigurator = new PipeConfigurator<NamespaceContext>();
            MessagingFactoryPipeConfigurator = new PipeConfigurator<MessagingFactoryContext>();
        }

        public IServiceBusBusConfiguration BusConfiguration => _hostConfiguration.BusConfiguration;
        public IServiceBusHostConfiguration HostConfiguration => _hostConfiguration;

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

        protected IReceiveEndpoint CreateReceiveEndpoint(ServiceBusReceiveEndpointContext context)
        {
            var transportObserver = context.TransportObservers;

            IAgent consumerAgent;
            if (_hostConfiguration.BusConfiguration.DeployTopologyOnly)
            {
                var transportReadyFilter = new TransportReadyFilter<ClientContext>(context);
                ClientPipeConfigurator.UseFilter(transportReadyFilter);

                consumerAgent = transportReadyFilter;
            }
            else
            {
                var messageReceiver = new BrokeredMessageReceiver(InputAddress,  context);

                var errorTransport = CreateErrorTransport(_hostConfiguration.Host);
                var deadLetterTransport = CreateDeadLetterTransport(_hostConfiguration.Host);

                context.GetOrAddPayload(() => deadLetterTransport);
                context.GetOrAddPayload(() => errorTransport);

                var receiverFilter = _settings.RequiresSession
                    ? new MessageSessionReceiverFilter(messageReceiver, transportObserver)
                    : new MessageReceiverFilter(messageReceiver, transportObserver);

                ClientPipeConfigurator.UseFilter(receiverFilter);

                consumerAgent = receiverFilter;
            }

            IPipe<ClientContext> clientPipe = ClientPipeConfigurator.Build();

            var clientCache = CreateClientCache(InputAddress, _hostConfiguration.Host.MessagingFactoryContextSupervisor, _hostConfiguration.Host.NamespaceContextSupervisor);

            var transport = new ReceiveTransport(_hostConfiguration.Host, _settings, clientCache, clientPipe, context);

            transport.Add(consumerAgent);

            return CreateReceiveEndpoint(_settings.Path, transport, context);
        }

        protected abstract IErrorTransport CreateErrorTransport(IServiceBusHostControl host);
        protected abstract IDeadLetterTransport CreateDeadLetterTransport(IServiceBusHostControl host);

        protected abstract IClientContextSupervisor CreateClientCache(Uri inputAddress, IMessagingFactoryContextSupervisor messagingFactoryContextSupervisor, INamespaceContextSupervisor namespaceContextSupervisor);

        protected abstract IPipeContextFactory<SendEndpointContext> CreateSendEndpointContextFactory(IServiceBusHost host, SendSettings settings,
            IPipe<NamespaceContext> namespacePipe);

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
