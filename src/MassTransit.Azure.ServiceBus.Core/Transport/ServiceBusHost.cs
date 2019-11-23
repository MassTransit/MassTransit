namespace MassTransit.Azure.ServiceBus.Core.Transport
{
    using System;
    using System.Threading.Tasks;
    using Configuration;
    using Context;
    using Contexts;
    using Definition;
    using GreenPipes;
    using GreenPipes.Agents;
    using GreenPipes.Caching;
    using MassTransit.Configurators;
    using Metadata;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Azure.ServiceBus.Management;
    using Pipeline;
    using Settings;
    using Topology;
    using Transports;


    public class ServiceBusHost :
        BaseHost,
        IServiceBusHostControl
    {
        readonly IServiceBusHostConfiguration _hostConfiguration;
        readonly IServiceBusHostTopology _hostTopology;
        readonly IIndex<Uri, CachedSendTransport> _index;

        public ServiceBusHost(IServiceBusHostConfiguration hostConfiguration, IServiceBusHostTopology hostTopology)
            : base(hostConfiguration, hostTopology)
        {
            _hostConfiguration = hostConfiguration;
            _hostTopology = hostTopology;

            RetryPolicy = Retry.CreatePolicy(x =>
            {
                x.Ignore<MessagingEntityNotFoundException>();
                x.Ignore<MessagingEntityAlreadyExistsException>();
                x.Ignore<MessageNotFoundException>();
                x.Ignore<MessageSizeExceededException>();

                x.Handle<ServerBusyException>(exception => exception.IsTransient);
                x.Handle<TimeoutException>();

                x.Interval(5, TimeSpan.FromSeconds(10));
            });

            MessagingFactoryContextSupervisor = new MessagingFactoryContextSupervisor(hostConfiguration);

            NamespaceContextSupervisor = new NamespaceContextSupervisor(hostConfiguration);

            var cacheSettings = new CacheSettings(SendEndpointCacheDefaults.Capacity, SendEndpointCacheDefaults.MinAge, SendEndpointCacheDefaults.MaxAge);

            var cache = new GreenCache<CachedSendTransport>(cacheSettings);
            _index = cache.AddIndex("key", x => x.Address);
        }

        public IMessagingFactoryContextSupervisor MessagingFactoryContextSupervisor { get; }
        public INamespaceContextSupervisor NamespaceContextSupervisor { get; }
        public IRetryPolicy RetryPolicy { get; }
        public ServiceBusHostSettings Settings => _hostConfiguration.Settings;
        public IServiceBusHostTopology Topology => _hostTopology;

        protected override void Probe(ProbeContext context)
        {
            context.Set(new
            {
                Type = "Azure Service Bus",
                _hostConfiguration.HostAddress,
                _hostConfiguration.Settings.OperationTimeout
            });

            NamespaceContextSupervisor.Probe(context);

            MessagingFactoryContextSupervisor.Probe(context);
        }

        public override HostReceiveEndpointHandle ConnectReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter,
            Action<IReceiveEndpointConfigurator> configureEndpoint = null)
        {
            return ConnectReceiveEndpoint(definition, endpointNameFormatter, configureEndpoint);
        }

        public override HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configureEndpoint = null)
        {
            return ConnectReceiveEndpoint(queueName, configureEndpoint);
        }

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter = null,
            Action<IServiceBusReceiveEndpointConfigurator> configureEndpoint = null)
        {
            var queueName = definition.GetEndpointName(endpointNameFormatter ?? DefaultEndpointNameFormatter.Instance);

            return ConnectReceiveEndpoint(queueName, x => x.Apply(definition, configureEndpoint));
        }

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IServiceBusReceiveEndpointConfigurator> configure = null)
        {
            LogContext.SetCurrentIfNull(DefaultLogContext);

            LogContext.Debug?.Log("Connect receive endpoint: {Queue}", queueName);

            var configuration = _hostConfiguration.CreateReceiveEndpointConfiguration(queueName, configure);

            BusConfigurationResult.CompileResults(configuration.Validate());

            configuration.Build(this);

            return ReceiveEndpoints.Start(configuration.Settings.Path);
        }

        public HostReceiveEndpointHandle ConnectSubscriptionEndpoint<T>(string subscriptionName,
            Action<IServiceBusSubscriptionEndpointConfigurator> configure = null)
            where T : class
        {
            var settings = new SubscriptionEndpointSettings(Topology.Publish<T>().TopicDescription, subscriptionName);

            return ConnectSubscriptionEndpoint(settings, configure);
        }

        public HostReceiveEndpointHandle ConnectSubscriptionEndpoint(string subscriptionName, string topicName,
            Action<IServiceBusSubscriptionEndpointConfigurator> configure = null)
        {
            var settings = new SubscriptionEndpointSettings(topicName, subscriptionName);

            return ConnectSubscriptionEndpoint(settings, configure);
        }

        HostReceiveEndpointHandle ConnectSubscriptionEndpoint(SubscriptionEndpointSettings settings,
            Action<IServiceBusSubscriptionEndpointConfigurator> configure)
        {
            LogContext.SetCurrentIfNull(DefaultLogContext);

            LogContext.Debug?.Log("Connect subscription endpoint: {Topic}/{SubscriptionName}", settings.Path, settings.Name);

            var configuration = _hostConfiguration.CreateSubscriptionEndpointConfiguration(settings, configure);

            BusConfigurationResult.CompileResults(configuration.Validate());

            configuration.Build(this);

            return ReceiveEndpoints.Start(configuration.Settings.Path);
        }

        public async Task<ISendTransport> CreateSendTransport(ServiceBusEndpointAddress address)
        {
            Task<CachedSendTransport> Create(Uri transportAddress)
            {
                var settings = _hostTopology.SendTopology.GetSendSettings(address);

                var endpointContextSupervisor = CreateQueueSendEndpointContextSupervisor(settings);

                var transportContext = new HostServiceBusSendTransportContext(address, endpointContextSupervisor, SendLogContext);

                var transport = new ServiceBusSendTransport(transportContext);
                Add(transport);

                return Task.FromResult(new CachedSendTransport(transportAddress, transport));
            }

            return await _index.Get(address, Create).ConfigureAwait(false);
        }

        public Task<ISendTransport> CreatePublishTransport<T>()
            where T : class
        {
            var publishTopology = _hostTopology.Publish<T>();

            if (!publishTopology.TryGetPublishAddress(_hostConfiguration.HostAddress, out Uri publishAddress))
                throw new ArgumentException($"The type did not return a valid publish address: {TypeMetadataCache<T>.ShortName}");

            var settings = publishTopology.GetSendSettings();

            var endpointContextSupervisor = CreateTopicSendEndpointContextSupervisor(settings);

            var transportContext = new HostServiceBusSendTransportContext(publishAddress, endpointContextSupervisor, SendLogContext);

            var transport = new ServiceBusSendTransport(transportContext);
            Add(transport);

            return Task.FromResult<ISendTransport>(transport);
        }

        ISendEndpointContextSupervisor CreateQueueSendEndpointContextSupervisor(SendSettings settings)
        {
            IPipe<NamespaceContext> namespacePipe = CreateConfigureTopologyPipe(settings);

            var contextFactory = new QueueSendEndpointContextFactory(MessagingFactoryContextSupervisor, NamespaceContextSupervisor,
                Pipe.Empty<MessagingFactoryContext>(), namespacePipe, settings);

            return new SendEndpointContextSupervisor(contextFactory);
        }

        ISendEndpointContextSupervisor CreateTopicSendEndpointContextSupervisor(SendSettings settings)
        {
            IPipe<NamespaceContext> namespacePipe = CreateConfigureTopologyPipe(settings);

            var contextFactory = new TopicSendEndpointContextFactory(MessagingFactoryContextSupervisor, NamespaceContextSupervisor,
                Pipe.Empty<MessagingFactoryContext>(), namespacePipe, settings);

            return new SendEndpointContextSupervisor(contextFactory);
        }

        IPipe<NamespaceContext> CreateConfigureTopologyPipe(SendSettings settings)
        {
            var configureTopologyFilter = new ConfigureTopologyFilter<SendSettings>(settings, settings.GetBrokerTopology(), false, Stopping);

            return configureTopologyFilter.ToPipe();
        }

        protected override IAgent[] GetAgentHandles()
        {
            return new IAgent[] {NamespaceContextSupervisor, MessagingFactoryContextSupervisor};
        }
    }
}
