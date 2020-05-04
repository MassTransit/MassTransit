namespace MassTransit.SignalR.Contracts
{
    using Microsoft.AspNetCore.SignalR;


    public interface Ack<THub>
        where THub : Hub
    {
        string ServerName { get; }
    }
}
