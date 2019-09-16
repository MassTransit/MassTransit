namespace MassTransit.AmazonSqsTransport.Configuration.Configuration
{
    using System;
    using System.Threading.Tasks;
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


    public class AmazonSqsHostConfiguration :
        IAmazonSqsHostConfiguration
    {
        readonly IAmazonSqsBusConfiguration _busConfiguration;
        readonly AmazonSqsHost _host;
        readonly IAmazonSqsHostTopology _topology;

        public AmazonSqsHostConfiguration(IAmazonSqsBusConfiguration busConfiguration, AmazonSqsHostSettings settings, IAmazonSqsHostTopology topology)
        {
            Settings = settings;
            _topology = topology;
            _busConfiguration = busConfiguration;

            _host = new AmazonSqsHost(this);
        }

        IBusHostControl IHostConfiguration.Host => _host;
        Uri IHostConfiguration.HostAddress => Settings.HostAddress;
        IHostTopology IHostConfiguration.Topology => _topology;

        public AmazonSqsHostSettings Settings { get; }

        IAmazonSqsBusConfiguration IAmazonSqsHostConfiguration.BusConfiguration => _busConfiguration;
        IAmazonSqsHostControl IAmazonSqsHostConfiguration.Host => _host;
        IAmazonSqsHostTopology IAmazonSqsHostConfiguration.Topology => _topology;

        public bool Matches(Uri address)
        {
            if (!address.Scheme.Equals("amazonsqs", StringComparison.OrdinalIgnoreCase))
                return false;

            var settings = new AmazonSqsHostConfigurator(address).Settings;

            return AmazonSqsHostEqualityComparer.Default.Equals(Settings, settings);
        }

        public Task<ISendTransport> CreateSendTransport(Uri address)
        {
            var settings = _topology.SendTopology.GetSendSettings(address);

            var clientContextSupervisor = new AmazonSqsClientContextSupervisor(_host.ConnectionContextSupervisor);

            var configureTopologyPipe = new ConfigureTopologyFilter<SendSettings>(settings, settings.GetBrokerTopology()).ToPipe();

            var transportContext = new HostSqsSendTransportContext(clientContextSupervisor, configureTopologyPipe, settings.EntityName, _host.SendLogContext);

            var transport = new QueueSendTransport(transportContext);
            _host.Add(transport);

            return Task.FromResult<ISendTransport>(transport);
        }

        public Task<ISendTransport> CreatePublishTransport<T>()
            where T : class
        {
            IAmazonSqsMessagePublishTopology<T> publishTopology = _topology.Publish<T>();

            var settings = publishTopology.GetPublishSettings();

            var clientContextSupervisor = new AmazonSqsClientContextSupervisor(_host.ConnectionContextSupervisor);

            var configureTopologyPipe = new ConfigureTopologyFilter<PublishSettings>(settings, publishTopology.GetBrokerTopology()).ToPipe();

            var transportContext = new HostSqsSendTransportContext(clientContextSupervisor, configureTopologyPipe, settings.EntityName, _host.SendLogContext);

            var transport = new TopicSendTransport(transportContext);
            _host.Add(transport);

            return Task.FromResult<ISendTransport>(transport);
        }

        public IAmazonSqsReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName)
        {
            return new AmazonSqsReceiveEndpointConfiguration(this, queueName, _busConfiguration.CreateEndpointConfiguration());
        }


        class HostSqsSendTransportContext :
            BaseSendTransportContext,
            SqsSendTransportContext
        {
            public HostSqsSendTransportContext(IClientContextSupervisor clientContextSupervisor, IPipe<ClientContext> configureTopologyPipe, string entityName,
                ILogContext logContext)
                : base(logContext)
            {
                ClientContextSupervisor = clientContextSupervisor;
                ConfigureTopologyPipe = configureTopologyPipe;
                EntityName = entityName;
            }

            public IPipe<ClientContext> ConfigureTopologyPipe { get; }
            public string EntityName { get; }
            public IClientContextSupervisor ClientContextSupervisor { get; }
        }
    }
}
