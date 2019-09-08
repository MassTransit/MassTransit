namespace MassTransit.ActiveMqTransport.Configuration
{
    using System;
    using System.Threading.Tasks;
    using Apache.NMS;
    using Configurators;
    using Context;
    using Contexts;
    using GreenPipes;
    using MassTransit.Configuration;
    using MassTransit.Topology;
    using Pipeline;
    using Topology;
    using Transport;
    using Transports;


    public class ActiveMqHostConfiguration :
        IActiveMqHostConfiguration
    {
        readonly IActiveMqBusConfiguration _busConfiguration;
        readonly ActiveMqHostSettings _hostSettings;
        readonly IActiveMqHostTopology _hostTopology;
        readonly ActiveMqHost _host;

        public ActiveMqHostConfiguration(IActiveMqBusConfiguration busConfiguration, ActiveMqHostSettings hostSettings, IActiveMqHostTopology hostTopology)
        {
            _busConfiguration = busConfiguration;
            _hostSettings = hostSettings;
            _hostTopology = hostTopology;

            _host = new ActiveMqHost(this);

            Description = hostSettings.ToDescription();
        }

        Uri IHostConfiguration.HostAddress => _hostSettings.HostAddress;
        IBusHostControl IHostConfiguration.Host => _host;
        IHostTopology IHostConfiguration.Topology => _hostTopology;

        IActiveMqBusConfiguration IActiveMqHostConfiguration.BusConfiguration => _busConfiguration;
        IActiveMqHostControl IActiveMqHostConfiguration.Host => _host;
        IActiveMqHostTopology IActiveMqHostConfiguration.Topology => _hostTopology;

        public string Description { get; }
        ActiveMqHostSettings IActiveMqHostConfiguration.Settings => _hostSettings;

        public bool Matches(Uri address)
        {
            if (!address.Scheme.Equals("activemq", StringComparison.OrdinalIgnoreCase))
                return false;

            var settings = new ActiveMqHostConfigurator(address).Settings;

            return ActiveMqHostEqualityComparer.Default.Equals(_hostSettings, settings);
        }

        public Task<ISendTransport> CreateSendTransport(Uri address)
        {
            var settings = _host.Topology.SendTopology.GetSendSettings(address);

            var sessionContextSupervisor = CreateSessionContextSupervisor();

            var configureTopology = new ConfigureTopologyFilter<SendSettings>(settings, settings.GetBrokerTopology()).ToPipe();

            return CreateSendTransport(sessionContextSupervisor, configureTopology, settings.EntityName, DestinationType.Queue);
        }

        public Task<ISendTransport> CreatePublishTransport<T>()
            where T : class
        {
            IActiveMqMessagePublishTopology<T> publishTopology = _hostTopology.Publish<T>();

            var settings = publishTopology.GetSendSettings();

            var sessionContextSupervisor = CreateSessionContextSupervisor();

            var configureTopology = new ConfigureTopologyFilter<SendSettings>(settings, publishTopology.GetBrokerTopology()).ToPipe();

            return CreateSendTransport(sessionContextSupervisor, configureTopology, settings.EntityName, DestinationType.Topic);
        }

        ISessionContextSupervisor CreateSessionContextSupervisor()
        {
            return new ActiveMqSessionContextSupervisor(_host.ConnectionContextSupervisor);
        }

        Task<ISendTransport> CreateSendTransport(ISessionContextSupervisor sessionContextSupervisor, IPipe<SessionContext> pipe,
            string entityName, DestinationType destinationType)
        {
            var sendTransportContext = new HostActiveMqSendTransportContext(sessionContextSupervisor, pipe, entityName, destinationType, _host.SendLogContext);

            var transport = new ActiveMqSendTransport(sendTransportContext);
            _host.Add(transport);

            return Task.FromResult<ISendTransport>(transport);
        }

        public IActiveMqReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName)
        {
            return new ActiveMqReceiveEndpointConfiguration(this, queueName, _busConfiguration.CreateEndpointConfiguration());
        }


        class HostActiveMqSendTransportContext :
            BaseSendTransportContext,
            ActiveMqSendTransportContext
        {
            public HostActiveMqSendTransportContext(ISessionContextSupervisor sessionContextSupervisor, IPipe<SessionContext> configureTopologyPipe, string
                entityName, DestinationType destinationType, ILogContext logContext)
                : base(logContext)
            {
                SessionContextSupervisor = sessionContextSupervisor;
                ConfigureTopologyPipe = configureTopologyPipe;
                EntityName = entityName;
                DestinationType = destinationType;
            }

            public IPipe<SessionContext> ConfigureTopologyPipe { get; }
            public string EntityName { get; }
            public DestinationType DestinationType { get; }
            public ISessionContextSupervisor SessionContextSupervisor { get; }
        }
    }
}
