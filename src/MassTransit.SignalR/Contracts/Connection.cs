namespace MassTransit.SignalR.Contracts
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.SignalR;


    public interface Connection<THub>
        where THub : Hub
    {
        string ConnectionId { get; }
        IReadOnlyDictionary<string, byte[]> Messages { get; }
    }
}
