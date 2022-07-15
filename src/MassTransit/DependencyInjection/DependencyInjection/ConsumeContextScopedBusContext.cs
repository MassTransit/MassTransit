#nullable enable
namespace MassTransit.DependencyInjection
{
    using System;
    using Clients;
    using Transports;


    public class ConsumeContextScopedBusContext :
        ScopedBusContext
    {
        readonly ScopedClientFactory _clientFactory;
        readonly ConsumeContext _context;

        public ConsumeContextScopedBusContext(ConsumeContext context, IClientFactory clientFactory)
        {
            _context = context;
            _clientFactory = new ScopedClientFactory(clientFactory, context);
        }

        public ISendEndpointProvider SendEndpointProvider => _context;

        public IPublishEndpoint PublishEndpoint => _context;

        public IScopedClientFactory ClientFactory => _clientFactory;
    }


    public class ConsumeContextScopedBusContext<TBus> :
        ScopedBusContext
        where TBus : class, IBus
    {
        readonly TBus _bus;
        readonly ScopedClientFactory _clientFactory;
        readonly ConsumeContext _context;
        readonly IServiceProvider _provider;
        IPublishEndpoint? _publishEndpoint;
        ISendEndpointProvider? _sendEndpointProvider;

        public ConsumeContextScopedBusContext(TBus bus, ConsumeContext context, IClientFactory clientFactory, IServiceProvider provider)
        {
            _bus = bus;
            _context = context;
            _provider = provider;
            _clientFactory = new ScopedClientFactory(clientFactory, context);
        }

        public ISendEndpointProvider SendEndpointProvider
        {
            get { return _sendEndpointProvider ??= new ScopedConsumeSendEndpointProvider<IServiceProvider>(_bus, _context, _provider); }
        }

        public IPublishEndpoint PublishEndpoint
        {
            get { return _publishEndpoint ??= new PublishEndpoint(new ScopedConsumePublishEndpointProvider<IServiceProvider>(_bus, _context, _provider)); }
        }

        public IScopedClientFactory ClientFactory => _clientFactory;
    }
}
