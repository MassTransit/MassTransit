namespace MassTransit.Transports.InMemory
{
    using System;
    using System.Threading.Tasks;
    using Builders;
    using Configuration;
    using Context;
    using Contexts;
    using Fabric;
    using GreenPipes;
    using GreenPipes.Agents;
    using Topology;
    using Topology.Builders;
    using Topology.Configurators;


    public class InMemoryTransportProvider :
        Supervisor,
        IInMemoryTransportProvider
    {
        readonly IInMemoryHostConfiguration _hostConfiguration;
        readonly Lazy<IMessageFabric> _messageFabric;
        readonly IInMemoryTopologyConfiguration _topologyConfiguration;

        public InMemoryTransportProvider(IInMemoryHostConfiguration hostConfiguration, IInMemoryTopologyConfiguration topologyConfiguration)
        {
            _hostConfiguration = hostConfiguration;
            _topologyConfiguration = topologyConfiguration;

            _messageFabric = new Lazy<IMessageFabric>(() => new MessageFabric(hostConfiguration.TransportConcurrencyLimit));
        }

        public IReceiveTransport GetReceiveTransport(string queueName, ReceiveEndpointContext receiveEndpointContext)
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.LogContext);

            TransportLogMessages.CreateReceiveTransport(receiveEndpointContext.InputAddress);

            var queue = _messageFabric.Value.GetQueue(queueName);

            IDeadLetterTransport deadLetterTransport = new InMemoryMessageDeadLetterTransport(_messageFabric.Value.GetExchange($"{queueName}_skipped"));
            receiveEndpointContext.GetOrAddPayload(() => deadLetterTransport);

            IErrorTransport errorTransport = new InMemoryMessageErrorTransport(_messageFabric.Value.GetExchange($"{queueName}_error"));
            receiveEndpointContext.GetOrAddPayload(() => errorTransport);

            var transport = new InMemoryReceiveTransport(new Uri(_hostConfiguration.HostAddress, queueName), queue, receiveEndpointContext);
            Add(transport);

            return transport;
        }

        public async Task<ISendTransport> GetSendTransport(Uri address)
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.LogContext);

            var endpointAddress = new InMemoryEndpointAddress(_hostConfiguration.HostAddress, address);

            TransportLogMessages.CreateSendTransport(address);

            var exchange = _messageFabric.Value.GetExchange(endpointAddress.Name);

            var context = new ExchangeInMemorySendTransportContext(exchange, _hostConfiguration.SendLogContext);

            return new InMemorySendTransport(context);
        }

        public Uri NormalizeAddress(Uri address)
        {
            return new InMemoryEndpointAddress(_hostConfiguration.HostAddress, address);
        }

        public IInMemoryConsumeTopologyBuilder CreateConsumeTopologyBuilder()
        {
            return new InMemoryConsumeTopologyBuilder(_messageFabric.Value);
        }

        public Task<ISendTransport> GetPublishTransport<T>(Uri publishAddress)
            where T : class
        {
            IInMemoryMessagePublishTopologyConfigurator<T> publishTopology = _topologyConfiguration.Publish.GetMessageTopology<T>();

            ApplyTopologyToMessageFabric(publishTopology);

            return GetSendTransport(publishAddress);
        }

        public void Probe(ProbeContext context)
        {
            _messageFabric.Value.Probe(context);
        }

        protected override async Task StopSupervisor(StopSupervisorContext context)
        {
            await base.StopSupervisor(context).ConfigureAwait(false);

            if (_messageFabric.IsValueCreated)
                await _messageFabric.Value.DisposeAsync().ConfigureAwait(false);
        }

        void ApplyTopologyToMessageFabric<T>(IInMemoryMessagePublishTopology<T> publishTopology)
            where T : class
        {
            publishTopology.Apply(new PublishEndpointTopologyBuilder(_messageFabric.Value));
        }
    }
}
