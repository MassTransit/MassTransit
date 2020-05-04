namespace MassTransit.SignalR.Tests
{
    using Contracts;
    using Microsoft.AspNetCore.SignalR;
    using Scoping;


    public class BusHubLifetimeScopeProvider :
        IHubLifetimeScopeProvider
    {
        readonly IBus _bus;
        readonly IClientFactory _clientFactory;

        public BusHubLifetimeScopeProvider(IBus bus)
        {
            _bus = bus;
            _clientFactory = bus.CreateClientFactory();
        }

        public IHubLifetimeScope<THub> CreateScope<THub>()
            where THub : Hub
        {
            return new HubLifetimeScope<THub>(_bus, _clientFactory);
        }


        class HubLifetimeScope<THub> :
            IHubLifetimeScope<THub>
            where THub : Hub
        {
            public HubLifetimeScope(IPublishEndpoint bus, IClientFactory clientFactory)
            {
                PublishEndpoint = bus;
                RequestClient = clientFactory.CreateRequestClient<GroupManagement<THub>>();
            }

            public IPublishEndpoint PublishEndpoint { get; }
            public IRequestClient<GroupManagement<THub>> RequestClient { get; }

            public void Dispose()
            {
            }
        }
    }
}
