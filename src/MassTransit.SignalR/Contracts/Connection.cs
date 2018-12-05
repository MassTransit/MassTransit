namespace MassTransit.SignalR.Contracts
{
    using Microsoft.AspNetCore.SignalR;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public interface Connection<THub> where THub : Hub
    {
        string ConnectionId { get; set; }
        IDictionary<string, byte[]> Messages { get; set; }
    }
}
