namespace MassTransit.SignalR.Contracts
{
    using Microsoft.AspNetCore.SignalR;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public interface Group<THub> where THub : Hub
    {
        string GroupName { get; set; }
        string[] ExcludedConnectionIds { get; set; }
        IDictionary<string, byte[]> Messages { get; set; }
    }
}
