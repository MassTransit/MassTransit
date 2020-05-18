namespace MassTransit.SignalR.Tests
{
    using Microsoft.AspNetCore.SignalR;


    public interface IHubManagerConsumerFactory<THub>
        where THub : Hub
    {
        MassTransitHubLifetimeManager<THub> HubLifetimeManager { get; set; }
    }
}
