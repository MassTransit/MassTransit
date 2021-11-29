namespace MassTransit.InMemoryTransport
{
    using System;
    using System.Threading.Tasks;
    using Configuration;
    using Fabric;
    using Middleware;
    using Transports;


    public sealed class InMemoryTransportProvider :
        Agent,
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

            SetReady();
        }

        public IMessageFabric MessageFabric => _messageFabric.Value;

        public async Task<ISendTransport> CreateSendTransport(ReceiveEndpointContext receiveEndpointContext, Uri address)
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.LogContext);

            var endpointAddress = new InMemoryEndpointAddress(_hostConfiguration.HostAddress, address);

            TransportLogMessages.CreateSendTransport(address);

            var exchange = _messageFabric.Value.GetExchange(endpointAddress.Name);

            var context = new ExchangeInMemorySendTransportContext(_hostConfiguration, receiveEndpointContext, exchange);

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

        public Task<ISendTransport> CreatePublishTransport<T>(ReceiveEndpointContext receiveEndpointContext, Uri publishAddress)
            where T : class
        {
            IInMemoryMessagePublishTopologyConfigurator<T> publishTopology = _topologyConfiguration.Publish.GetMessageTopology<T>();

            ApplyTopologyToMessageFabric(publishTopology);

            return CreateSendTransport(receiveEndpointContext, publishAddress);
        }

        public void Probe(ProbeContext context)
        {
            if (_messageFabric.IsValueCreated)
                _messageFabric.Value.Probe(context);
        }

        protected override async Task StopAgent(StopContext context)
        {
            await base.StopAgent(context).ConfigureAwait(false);

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
