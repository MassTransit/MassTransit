namespace MassTransit.Transports.InMemory
{
    using System;
    using System.Threading.Tasks;
    using Builders;
    using Configuration;
    using Context;
    using Contexts;
    using Definition;
    using Fabric;
    using GreenPipes;
    using MassTransit.Configurators;
    using Topology.Builders;
    using Topology.Topologies;


    /// <summary>
    /// Caches InMemory transport instances so that they are only created and used once
    /// </summary>
    public class InMemoryHost :
        BaseHost,
        IInMemoryHostControl
    {
        readonly IInMemoryHostConfiguration _hostConfiguration;
        readonly IMessageFabric _messageFabric;

        public InMemoryHost(IInMemoryHostConfiguration hostConfiguration, IInMemoryHostTopology hostTopology)
            : base(hostConfiguration, hostTopology)
        {
            _hostConfiguration = hostConfiguration;

            _messageFabric = new MessageFabric(hostConfiguration.TransportConcurrencyLimit);
        }

        public IReceiveTransport GetReceiveTransport(string queueName, ReceiveEndpointContext receiveEndpointContext)
        {
            LogContext.SetCurrentIfNull(DefaultLogContext);

            TransportLogMessages.CreateReceiveTransport(receiveEndpointContext.InputAddress);

            var queue = _messageFabric.GetQueue(queueName);

            IDeadLetterTransport deadLetterTransport = new InMemoryMessageDeadLetterTransport(_messageFabric.GetExchange($"{queueName}_skipped"));
            receiveEndpointContext.GetOrAddPayload(() => deadLetterTransport);

            IErrorTransport errorTransport = new InMemoryMessageErrorTransport(_messageFabric.GetExchange($"{queueName}_error"));
            receiveEndpointContext.GetOrAddPayload(() => errorTransport);

            var transport = new InMemoryReceiveTransport(new Uri(_hostConfiguration.HostAddress, queueName), queue, receiveEndpointContext);
            Add(transport);

            return transport;
        }

        public override HostReceiveEndpointHandle ConnectReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter,
            Action<IReceiveEndpointConfigurator> configureEndpoint = null)
        {
            return ConnectReceiveEndpoint(definition, endpointNameFormatter, configureEndpoint);
        }

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter,
            Action<IInMemoryReceiveEndpointConfigurator> configureEndpoint = null)
        {
            var queueName = definition.GetEndpointName(endpointNameFormatter ?? DefaultEndpointNameFormatter.Instance);

            return ConnectReceiveEndpoint(queueName, configurator =>
            {
                _hostConfiguration.ApplyEndpointDefinition(configurator, definition);
                configureEndpoint?.Invoke(configurator);
            });
        }

        public override HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configureEndpoint = null)
        {
            return ConnectReceiveEndpoint(queueName, configureEndpoint);
        }

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IInMemoryReceiveEndpointConfigurator> configure = null)
        {
            LogContext.SetCurrentIfNull(DefaultLogContext);

            var configuration = _hostConfiguration.CreateReceiveEndpointConfiguration(queueName, configure);

            TransportLogMessages.ConnectReceiveEndpoint(configuration.InputAddress);

            BusConfigurationResult.CompileResults(configuration.Validate());

            configuration.Build(this);

            return ReceiveEndpoints.Start(queueName);
        }

        public async Task<ISendTransport> GetSendTransport(Uri address)
        {
            LogContext.SetCurrentIfNull(DefaultLogContext);

            var endpointAddress = new InMemoryEndpointAddress(_hostConfiguration.HostAddress, address);

            TransportLogMessages.CreateSendTransport(address);

            var exchange = _messageFabric.GetExchange(endpointAddress.Name);

            var context = new ExchangeInMemorySendTransportContext(exchange, SendLogContext);

            return new InMemorySendTransport(context);
        }

        public Uri NormalizeAddress(Uri address)
        {
            return new InMemoryEndpointAddress(_hostConfiguration.HostAddress, address);
        }

        IInMemoryPublishTopologyBuilder IInMemoryHostControl.CreatePublishTopologyBuilder(PublishEndpointTopologyBuilder.Options options)
        {
            return new PublishEndpointTopologyBuilder(_messageFabric, options);
        }

        IInMemoryConsumeTopologyBuilder IInMemoryHostControl.CreateConsumeTopologyBuilder()
        {
            return new InMemoryConsumeTopologyBuilder(_messageFabric);
        }

        protected override void Probe(ProbeContext context)
        {
            context.Add("type", "InMemory");
            context.Add("baseAddress", _hostConfiguration.HostAddress);

            _messageFabric.Probe(context);
        }
    }
}
