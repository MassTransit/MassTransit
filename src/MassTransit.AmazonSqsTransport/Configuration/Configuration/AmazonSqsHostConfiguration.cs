namespace MassTransit.AmazonSqsTransport.Configuration.Configuration
{
    using System;
    using System.Threading.Tasks;
    using Configurators;
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
        readonly IAmazonSqsHostControl _host;
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

            var configureTopologyFilter = new ConfigureTopologyFilter<SendSettings>(settings, settings.GetBrokerTopology());

            var transport = new QueueSendTransport(clientContextSupervisor, configureTopologyFilter, settings.EntityName);
            transport.Add(clientContextSupervisor);

            _host.Add(transport);

            return Task.FromResult<ISendTransport>(transport);
        }

        public Task<ISendTransport> CreatePublishTransport<T>()
            where T : class
        {
            IAmazonSqsMessagePublishTopology<T> publishTopology = _topology.Publish<T>();

            var sendSettings = publishTopology.GetPublishSettings();

            var clientContextSupervisor = new AmazonSqsClientContextSupervisor(_host.ConnectionContextSupervisor);

            var configureTopologyFilter = new ConfigureTopologyFilter<PublishSettings>(sendSettings, publishTopology.GetBrokerTopology());

            var sendTransport = new TopicSendTransport(clientContextSupervisor, configureTopologyFilter, sendSettings.EntityName);
            sendTransport.Add(clientContextSupervisor);

            _host.Add(sendTransport);

            return Task.FromResult<ISendTransport>(sendTransport);
        }

        public IAmazonSqsReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName)
        {
            return new AmazonSqsReceiveEndpointConfiguration(this, queueName, _busConfiguration.CreateEndpointConfiguration());
        }
    }
}
