namespace MassTransit.SignalR.Scoping
{
    using System;
    using Contracts;
    using Microsoft.AspNetCore.SignalR;


    public interface IHubLifetimeScope<THub> :
        IDisposable
        where THub : Hub
    {
        IPublishEndpoint PublishEndpoint { get; }
        IRequestClient<GroupManagement<THub>> RequestClient { get; }
    }
}
