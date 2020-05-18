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

            ConnectionContextSupervisor = new ConnectionContextSupervisor(hostConfiguration);

            var cacheSettings = new CacheSettings(SendEndpointCacheDefaults.Capacity, SendEndpointCacheDefaults.MinAge, SendEndpointCacheDefaults.MaxAge);

            var cache = new GreenCache<CachedSendTransport>(cacheSettings);
            _index = cache.AddIndex("key", x => x.Address);
        }

        public IConnectionContextSupervisor ConnectionContextSupervisor { get; }
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

            ConnectionContextSupervisor.Probe(context);
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

            return ConnectReceiveEndpoint(queueName, configurator =>
            {
                _hostConfiguration.ApplyEndpointDefinition(configurator, definition);
                configureEndpoint?.Invoke(configurator);
            });
        }

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IServiceBusReceiveEndpointConfigurator> configure = null)
        {
            LogContext.SetCurrentIfNull(DefaultLogContext);

            var configuration = _hostConfiguration.CreateReceiveEndpointConfiguration(queueName, configure);

            BusConfigurationResult.CompileResults(configuration.Validate());

            TransportLogMessages.ConnectReceiveEndpoint(configuration.InputAddress);

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
                TransportLogMessages.CreateSendTransport(address);

                var settings = _hostTopology.SendTopology.GetSendSettings(address);

                var transport = CreateSendTransport(address, settings);

                return Task.FromResult(new CachedSendTransport(transportAddress, transport));
            }

            return await _index.Get(address, Create).ConfigureAwait(false);
        }

        public Task<ISendTransport> CreatePublishTransport<T>()
            where T : class
        {
            var publishTopology = _hostTopology.Publish<T>();

            if (!publishTopology.TryGetPublishAddress(_hostConfiguration.HostAddress, out var publishAddress))
                throw new ArgumentException($"The type did not return a valid publish address: {TypeMetadataCache<T>.ShortName}");

            var settings = publishTopology.GetSendSettings();

            var transport = CreateSendTransport(publishAddress, settings);

            return Task.FromResult(transport);
        }

        ISendTransport CreateSendTransport(Uri address, SendSettings settings)
        {
            var endpointContextSupervisor = CreateSendEndpointContextSupervisor(settings);

            var transportContext = new HostServiceBusSendTransportContext(address, endpointContextSupervisor, SendLogContext);

            var transport = new ServiceBusSendTransport(transportContext);
            Add(transport);

            return transport;
        }

        ISendEndpointContextSupervisor CreateSendEndpointContextSupervisor(SendSettings settings)
        {
            var topologyPipe = new ConfigureTopologyFilter<SendSettings>(settings, settings.GetBrokerTopology(), false, Stopping);

            var contextFactory = new SendEndpointContextFactory(ConnectionContextSupervisor, topologyPipe.ToPipe<SendEndpointContext>(), settings);

            return new SendEndpointContextSupervisor(contextFactory);
        }

        protected override IAgent[] GetAgentHandles()
        {
            return new IAgent[] {ConnectionContextSupervisor};
        }
    }
}
