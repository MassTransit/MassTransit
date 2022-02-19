namespace MassTransit.InMemoryTransport
{
    using System;
    using System.Threading.Tasks;
    using Configuration;
    using Middleware;
    using Transports;
    using Transports.Fabric;


    public sealed class InMemoryTransportProvider :
        Agent,
        IInMemoryTransportProvider
    {
        readonly IInMemoryHostConfiguration _hostConfiguration;
        readonly IMessageFabric<InMemoryTransportContext, InMemoryTransportMessage> _messageFabric;
        readonly IInMemoryTopologyConfiguration _topologyConfiguration;

        public InMemoryTransportProvider(IInMemoryHostConfiguration hostConfiguration, IInMemoryTopologyConfiguration topologyConfiguration)
        {
            _hostConfiguration = hostConfiguration;
            _topologyConfiguration = topologyConfiguration;

            _messageFabric = new MessageFabric<InMemoryTransportContext, InMemoryTransportMessage>();

            SetReady();
        }

        public IMessageFabric<InMemoryTransportContext, InMemoryTransportMessage> MessageFabric => _messageFabric;

        public async Task<ISendTransport> CreateSendTransport(ReceiveEndpointContext receiveEndpointContext, Uri address)
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.LogContext);

            var endpointAddress = new InMemoryEndpointAddress(_hostConfiguration.HostAddress, address);

            TransportLogMessages.CreateSendTransport(address);

            IMessageExchange<InMemoryTransportMessage> exchange = _messageFabric.GetExchange(this, endpointAddress.Name, endpointAddress.ExchangeType);

            var context = new ExchangeInMemorySendTransportContext(_hostConfiguration, receiveEndpointContext, exchange);

            return new InMemorySendTransport(context);
        }

        public Uri NormalizeAddress(Uri address)
        {
            return new InMemoryEndpointAddress(_hostConfiguration.HostAddress, address);
        }

        public Task<ISendTransport> CreatePublishTransport<T>(ReceiveEndpointContext receiveEndpointContext, Uri publishAddress)
            where T : class
        {
            IInMemoryMessagePublishTopologyConfigurator<T> publishTopology = _topologyConfiguration.Publish.GetMessageTopology<T>();

            ApplyTopologyToMessageFabric(publishTopology);

            return CreateSendTransport(receiveEndpointContext, publishAddress);
        }

        public void Probe(ProbeContext context)
        {
            _messageFabric.Probe(context);
        }

        protected override async Task StopAgent(StopContext context)
        {
            await base.StopAgent(context).ConfigureAwait(false);

            await _messageFabric.Stop(context).ConfigureAwait(false);
        }

        void ApplyTopologyToMessageFabric<T>(IInMemoryMessagePublishTopology<T> publishTopology)
            where T : class
        {
            publishTopology.Apply(new MessageFabricPublishTopologyBuilder<InMemoryTransportContext, InMemoryTransportMessage>(this, _messageFabric));
        }
    }
}
