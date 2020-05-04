namespace MassTransit.SignalR.Contracts
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.SignalR;


    public interface All<THub>
        where THub : Hub
    {
        string[] ExcludedConnectionIds { get; }
        IReadOnlyDictionary<string, byte[]> Messages { get; }
    }
}
