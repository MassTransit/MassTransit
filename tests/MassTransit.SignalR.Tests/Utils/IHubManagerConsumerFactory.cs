namespace MassTransit.SignalR.Tests.Utils
{
    using Microsoft.AspNetCore.SignalR;


    public interface IHubManagerConsumerFactory<THub>
        where THub : Hub
    {
        MassTransitHubLifetimeManager<THub> HubLifetimeManager { get; set; }
    }
}
