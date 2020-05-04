namespace MassTransit.SignalR.Scoping
{
    using System;
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
            return new HubLifetimeScope<THub>(_serviceScopeFactory.CreateScope());
        }


        class HubLifetimeScope<THub> :
            IHubLifetimeScope<THub>
            where THub : Hub
        {
            readonly IServiceScope _serviceScope;
            IServiceProvider ServiceProvider => _serviceScope.ServiceProvider;

            public HubLifetimeScope(IServiceScope serviceScope)
            {
                _serviceScope = serviceScope;
                PublishEndpoint = ServiceProvider.GetRequiredService<IPublishEndpoint>();
                RequestClient = ServiceProvider.GetRequiredService<IRequestClient<GroupManagement<THub>>>();
            }

            public IPublishEndpoint PublishEndpoint { get; }
            public IRequestClient<GroupManagement<THub>> RequestClient { get; }

            public void Dispose()
            {
                _serviceScope.Dispose();
            }
        }
    }
}
