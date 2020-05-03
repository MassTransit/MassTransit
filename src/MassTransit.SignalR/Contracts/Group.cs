namespace MassTransit.SignalR.Contracts
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.SignalR;


    public interface Group<THub> where THub : Hub
    {
        string GroupName { get; set; }
        string[] ExcludedConnectionIds { get; set; }
        IDictionary<string, byte[]> Messages { get; set; }
    }
}
