namespace MassTransit.ActiveMqTransport.Transport
{
    using System;
    using System.Threading.Tasks;
    using Apache.NMS;
    using Configuration;
    using Context;
    using Contexts;
    using Definition;
    using GreenPipes;
    using GreenPipes.Agents;
    using MassTransit.Configurators;
    using Pipeline;
    using Topology;
    using Transports;


    public class ActiveMqHost :
        BaseHost,
        IActiveMqHostControl
    {
        readonly IActiveMqHostConfiguration _hostConfiguration;
        readonly IActiveMqHostTopology _hostTopology;

        public ActiveMqHost(IActiveMqHostConfiguration hostConfiguration, IActiveMqHostTopology hostTopology)
            : base(hostConfiguration, hostTopology)
        {
            _hostConfiguration = hostConfiguration;
            _hostTopology = hostTopology;

            ConnectionRetryPolicy = Retry.CreatePolicy(x =>
            {
                x.Handle<ActiveMqTransportException>();

                x.Exponential(1000, TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(3));
            });

            ConnectionContextSupervisor = new ActiveMqConnectionContextSupervisor(hostConfiguration, hostTopology);
        }

        public IConnectionContextSupervisor ConnectionContextSupervisor { get; }
        public IRetryPolicy ConnectionRetryPolicy { get; }
        public ActiveMqHostSettings Settings => _hostConfiguration.Settings;
        IActiveMqHostTopology IActiveMqHost.Topology => _hostTopology;

        protected override void Probe(ProbeContext context)
        {
            context.Set(new
            {
                Type = "ActiveMQ",
                Settings.Host,
                Settings.Port,
                Settings.Username,
                Password = new string('*', Settings.Password.Length)
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
            Action<IActiveMqReceiveEndpointConfigurator> configureEndpoint = null)
        {
            var queueName = definition.GetEndpointName(endpointNameFormatter ?? DefaultEndpointNameFormatter.Instance);

            return ConnectReceiveEndpoint(queueName, x => x.Apply(definition, configureEndpoint));
        }

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IActiveMqReceiveEndpointConfigurator> configure = null)
        {
            LogContext.SetCurrentIfNull(DefaultLogContext);

            LogContext.Debug?.Log("Connect receive endpoint: {Queue}", queueName);

            var configuration = _hostConfiguration.CreateReceiveEndpointConfiguration(queueName, configure);

            BusConfigurationResult.CompileResults(configuration.Validate());

            configuration.Build(this);

            return ReceiveEndpoints.Start(queueName);
        }

        public Task<ISendTransport> CreateSendTransport(ActiveMqEndpointAddress address)
        {
            var settings = _hostTopology.SendTopology.GetSendSettings(address);

            var sessionContextSupervisor = CreateSessionContextSupervisor();

            var configureTopology = new ConfigureTopologyFilter<SendSettings>(settings, settings.GetBrokerTopology()).ToPipe();

            return CreateSendTransport(sessionContextSupervisor, configureTopology, settings.EntityName, DestinationType.Queue);
        }

        public Task<ISendTransport> CreatePublishTransport<T>()
            where T : class
        {
            IActiveMqMessagePublishTopology<T> publishTopology = _hostTopology.Publish<T>();

            var settings = publishTopology.GetSendSettings(_hostConfiguration.HostAddress);

            var sessionContextSupervisor = CreateSessionContextSupervisor();

            var configureTopology = new ConfigureTopologyFilter<SendSettings>(settings, publishTopology.GetBrokerTopology()).ToPipe();

            return CreateSendTransport(sessionContextSupervisor, configureTopology, settings.EntityName, DestinationType.Topic);
        }

        ISessionContextSupervisor CreateSessionContextSupervisor()
        {
            return new ActiveMqSessionContextSupervisor(ConnectionContextSupervisor);
        }

        Task<ISendTransport> CreateSendTransport(ISessionContextSupervisor sessionContextSupervisor, IPipe<SessionContext> pipe,
            string entityName, DestinationType destinationType)
        {
            var sendTransportContext = new HostActiveMqSendTransportContext(sessionContextSupervisor, pipe, entityName, destinationType, SendLogContext);

            var transport = new ActiveMqSendTransport(sendTransportContext);
            Add(transport);

            return Task.FromResult<ISendTransport>(transport);
        }

        protected override IAgent[] GetAgentHandles()
        {
            return new IAgent[] {ConnectionContextSupervisor};
        }
    }
}
