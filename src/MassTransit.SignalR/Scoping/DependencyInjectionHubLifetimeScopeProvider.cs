namespace MassTransit.SignalR.Scoping
{
    using System;
    using System.Threading.Tasks;
    using Contracts;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.Extensions.DependencyInjection;


    public class DependencyInjectionHubLifetimeScopeProvider :
        IHubLifetimeScopeProvider
    {
        readonly IServiceScopeFactory _serviceScopeFactory;

        public DependencyInjectionHubLifetimeScopeProvider(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public IHubLifetimeScope<THub> CreateScope<THub>()
            where THub : Hub
        {
            return new HubLifetimeScope<THub>(_serviceScopeFactory.CreateAsyncScope());
        }


        class HubLifetimeScope<THub> :
            IHubLifetimeScope<THub>
            where THub : Hub
        {
            readonly AsyncServiceScope _serviceScope;

            public HubLifetimeScope(AsyncServiceScope serviceScope)
            {
                _serviceScope = serviceScope;
                PublishEndpoint = ServiceProvider.GetRequiredService<IPublishEndpoint>();
                RequestClient = ServiceProvider.GetRequiredService<IRequestClient<GroupManagement<THub>>>();
            }

            IServiceProvider ServiceProvider => _serviceScope.ServiceProvider;

            public IPublishEndpoint PublishEndpoint { get; }
            public IRequestClient<GroupManagement<THub>> RequestClient { get; }

            public ValueTask DisposeAsync()
            {
                return _serviceScope.DisposeAsync();
            }
        }
    }
}
