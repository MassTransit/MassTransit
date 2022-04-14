namespace MassTransit.DependencyInjection
{
    using Clients;


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
}
