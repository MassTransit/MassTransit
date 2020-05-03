namespace MassTransit.SignalR
{
    using Microsoft.AspNetCore.SignalR;


    public interface IHubLifetimeManagerOptions
    {
        bool UseMessageData { set; }
        string ServerName { set; }
        RequestTimeout RequestTimeout { set; }
    }


    public interface IHubLifetimeManagerOptions<THub> :
        IHubLifetimeManagerOptions
        where THub : Hub
    {
    }
}
