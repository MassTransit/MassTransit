namespace MassTransit.RabbitMqTransport.Transport
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
    using Integration;
    using MassTransit.Configurators;
    using Pipeline;
    using Topology;
    using Transports;


    public class RabbitMqHost :
        BaseHost,
        IRabbitMqHostControl
    {
        readonly IRabbitMqHostConfiguration _hostConfiguration;
        readonly IRabbitMqHostTopology _hostTopology;
        readonly IIndex<Uri, CachedSendTransport> _index;

        public RabbitMqHost(IRabbitMqHostConfiguration hostConfiguration, IRabbitMqHostTopology hostTopology)
            : base(hostConfiguration, hostTopology)
        {
            _hostConfiguration = hostConfiguration;
            _hostTopology = hostTopology;

            ConnectionRetryPolicy = Retry.CreatePolicy(x =>
            {
                x.Handle<RabbitMqConnectionException>();

                x.Exponential(1000, TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(3));
            });

            ConnectionContextSupervisor = new RabbitMqConnectionContextSupervisor(hostConfiguration, hostTopology);

            var cacheSettings = new CacheSettings(SendEndpointCacheDefaults.Capacity, SendEndpointCacheDefaults.MinAge, SendEndpointCacheDefaults.MaxAge);

            var cache = new GreenCache<CachedSendTransport>(cacheSettings);
            _index = cache.AddIndex("key", x => x.Address);
        }

        protected override void Probe(ProbeContext context)
        {
            context.Set(new
            {
                Type = "RabbitMQ",
                Settings.Host,
                Settings.Port,
                Settings.VirtualHost,
                Settings.Username,
                Password = new string('*', Settings.Password.Length),
                Settings.Heartbeat,
                Settings.Ssl
            });

            if (Settings.Ssl)
            {
                context.Set(new {Settings.SslServerName});
            }

            ConnectionContextSupervisor.Probe(context);
        }

        public IConnectionContextSupervisor ConnectionContextSupervisor { get; }
        public IRetryPolicy ConnectionRetryPolicy { get; }
        public RabbitMqHostSettings Settings => _hostConfiguration.Settings;
        IRabbitMqHostTopology IRabbitMqHost.Topology => _hostTopology;

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
            Action<IRabbitMqReceiveEndpointConfigurator> configureEndpoint = null)
        {
            var queueName = definition.GetEndpointName(endpointNameFormatter ?? DefaultEndpointNameFormatter.Instance);

            return ConnectReceiveEndpoint(queueName, x => x.Apply(definition, configureEndpoint));
        }

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IRabbitMqReceiveEndpointConfigurator> configure = null)
        {
            LogContext.SetCurrentIfNull(DefaultLogContext);

            LogContext.Debug?.Log("Connect receive endpoint: {Queue}", queueName);

            var configuration = _hostConfiguration.CreateReceiveEndpointConfiguration(queueName, configure);

            BusConfigurationResult.CompileResults(configuration.Validate());

            configuration.Build(this);

            return ReceiveEndpoints.Start(queueName);
        }

        public async Task<ISendTransport> CreateSendTransport(RabbitMqEndpointAddress address)
        {
            Task<CachedSendTransport> Create(Uri transportAddress)
            {
                var settings = _hostTopology.SendTopology.GetSendSettings(address);

                var brokerTopology = settings.GetBrokerTopology();

                var supervisor = CreateModelContextSupervisor();

                IPipe<ModelContext> pipe = new ConfigureTopologyFilter<SendSettings>(settings, brokerTopology).ToPipe();

                var transport = CreateSendTransport(supervisor, pipe, settings.ExchangeName);

                return Task.FromResult(new CachedSendTransport(transportAddress, transport));
            }

            return await _index.Get(address, Create).ConfigureAwait(false);
        }

        public Task<ISendTransport> CreatePublishTransport<T>()
            where T : class
        {
            IRabbitMqMessagePublishTopology<T> publishTopology = _hostTopology.Publish<T>();

            var sendSettings = publishTopology.GetSendSettings(_hostConfiguration.HostAddress);

            var brokerTopology = publishTopology.GetBrokerTopology();

            var supervisor = CreateModelContextSupervisor();

            IPipe<ModelContext> pipe = new ConfigureTopologyFilter<SendSettings>(sendSettings, brokerTopology).ToPipe();

            var transport = CreateSendTransport(supervisor, pipe, publishTopology.Exchange.ExchangeName);

            return Task.FromResult(transport);
        }

        ISendTransport CreateSendTransport(IModelContextSupervisor modelContextSupervisor, IPipe<ModelContext> pipe, string exchangeName)
        {
            var sendTransportContext = new HostRabbitMqSendTransportContext(modelContextSupervisor, pipe, exchangeName, SendLogContext);

            var transport = new RabbitMqSendTransport(sendTransportContext);
            Add(transport);

            return transport;
        }

        IModelContextSupervisor CreateModelContextSupervisor()
        {
            return new ModelContextSupervisor(ConnectionContextSupervisor);
        }

        protected override IAgent[] GetAgentHandles()
        {
            return new IAgent[] {ConnectionContextSupervisor};
        }
    }
}
