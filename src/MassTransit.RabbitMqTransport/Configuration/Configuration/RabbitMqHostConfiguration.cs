namespace MassTransit.RabbitMqTransport.Configuration
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using Context;
    using Contexts;
    using GreenPipes;
    using Integration;
    using MassTransit.Configuration;
    using MassTransit.Topology;
    using Pipeline;
    using Topology;
    using Topology.Settings;
    using Transport;
    using Transports;


    public class RabbitMqHostConfiguration :
        IRabbitMqHostConfiguration
    {
        readonly IRabbitMqBusConfiguration _busConfiguration;
        readonly RabbitMqHost _host;
        readonly RabbitMqHostSettings _hostSettings;
        readonly IRabbitMqHostTopology _hostTopology;

        public RabbitMqHostConfiguration(IRabbitMqBusConfiguration busConfiguration, RabbitMqHostSettings hostSettings, IRabbitMqHostTopology hostTopology)
        {
            _busConfiguration = busConfiguration;
            _hostSettings = hostSettings;
            _hostTopology = hostTopology;

            _host = new RabbitMqHost(this);

            Description = FormatDescription(hostSettings);
        }

        public string Description { get; }

        Uri IHostConfiguration.HostAddress => _hostSettings.HostAddress;
        IBusHostControl IHostConfiguration.Host => _host;
        IHostTopology IHostConfiguration.Topology => _hostTopology;

        IRabbitMqBusConfiguration IRabbitMqHostConfiguration.BusConfiguration => _busConfiguration;
        IRabbitMqHostControl IRabbitMqHostConfiguration.Host => _host;

        public IRabbitMqReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName)
        {
            var settings = new RabbitMqReceiveSettings(queueName, _busConfiguration.Topology.Consume.ExchangeTypeSelector.DefaultExchangeType, true, false);

            return new RabbitMqReceiveEndpointConfiguration(this, settings, _busConfiguration.CreateEndpointConfiguration());
        }

        public IRabbitMqReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(RabbitMqReceiveSettings settings,
            IRabbitMqEndpointConfiguration endpointConfiguration)
        {
            return new RabbitMqReceiveEndpointConfiguration(this, settings, endpointConfiguration);
        }

        public bool Matches(Uri address)
        {
            switch (address.Scheme.ToLowerInvariant())
            {
                case "rabbitmq":
                case "amqp":
                case "rabbitmqs":
                case "amqps":
                    break;

                default:
                    return false;
            }

            var settings = address.GetHostSettings();

            return RabbitMqHostEqualityComparer.Default.Equals(_hostSettings, settings);
        }

        IRabbitMqHostTopology IRabbitMqHostConfiguration.Topology => _hostTopology;
        public bool PublisherConfirmation => _hostSettings.PublisherConfirmation;
        public RabbitMqHostSettings Settings => _hostSettings;

        public Task<ISendTransport> CreateSendTransport(Uri address)
        {
            var settings = _hostTopology.GetSendSettings(address);

            var brokerTopology = settings.GetBrokerTopology();

            var supervisor = CreateModelContextSupervisor();

            IPipe<ModelContext> pipe = new ConfigureTopologyFilter<SendSettings>(settings, brokerTopology).ToPipe();

            var transport = CreateSendTransport(supervisor, pipe, settings.ExchangeName);

            return Task.FromResult(transport);
        }

        public Task<ISendTransport> CreatePublishTransport<T>()
            where T : class
        {
            IRabbitMqMessagePublishTopology<T> publishTopology = _hostTopology.Publish<T>();

            var sendSettings = publishTopology.GetSendSettings();

            var brokerTopology = publishTopology.GetBrokerTopology();

            var supervisor = CreateModelContextSupervisor();

            IPipe<ModelContext> pipe = new ConfigureTopologyFilter<SendSettings>(sendSettings, brokerTopology).ToPipe();

            var transport = CreateSendTransport(supervisor, pipe, publishTopology.Exchange.ExchangeName);

            return Task.FromResult(transport);
        }

        ISendTransport CreateSendTransport(IModelContextSupervisor modelContextSupervisor, IPipe<ModelContext> pipe, string exchangeName)
        {
            var sendTransportContext = new HostRabbitMqSendTransportContext(modelContextSupervisor, pipe, exchangeName, _host.SendLogContext);

            var transport = new RabbitMqSendTransport(sendTransportContext);
            _host.Add(transport);

            return transport;
        }

        IModelContextSupervisor CreateModelContextSupervisor()
        {
            return new ModelContextSupervisor(_host.ConnectionContextSupervisor);
        }

        static string FormatDescription(RabbitMqHostSettings settings)
        {
            var sb = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(settings.Username))
                sb.Append(settings.Username).Append('@');

            sb.Append(settings.Host);

            var actualHost = settings.HostNameSelector?.LastHost;
            if (!string.IsNullOrWhiteSpace(actualHost))
                sb.Append('(').Append(actualHost).Append(')');

            if (settings.Port != -1)
                sb.Append(':').Append(settings.Port);

            if (string.IsNullOrWhiteSpace(settings.VirtualHost))
                sb.Append('/');
            else if (settings.VirtualHost.StartsWith("/"))
                sb.Append(settings.VirtualHost);
            else
                sb.Append("/").Append(settings.VirtualHost);

            return sb.ToString();
        }


        class HostRabbitMqSendTransportContext :
            BaseSendTransportContext,
            RabbitMqSendTransportContext
        {
            public HostRabbitMqSendTransportContext(IModelContextSupervisor modelContextSupervisor, IPipe<ModelContext> configureTopologyPipe, string exchange,
                ILogContext logContext)
                : base(logContext)
            {
                ModelContextSupervisor = modelContextSupervisor;
                ConfigureTopologyPipe = configureTopologyPipe;
                Exchange = exchange;
            }

            public IPipe<ModelContext> ConfigureTopologyPipe { get; }
            public string Exchange { get; }
            public IModelContextSupervisor ModelContextSupervisor { get; }
        }
    }
}
