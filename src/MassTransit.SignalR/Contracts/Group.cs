namespace MassTransit.SignalR.Contracts
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.SignalR;


    public interface Group<THub>
        where THub : Hub
    {
        string GroupName { get; }
        string[] ExcludedConnectionIds { get; }
        IReadOnlyDictionary<string, byte[]> Messages { get; }
    }
}
