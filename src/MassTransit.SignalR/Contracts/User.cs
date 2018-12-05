namespace MassTransit.SignalR.Contracts
{
    using Microsoft.AspNetCore.SignalR;
    using System.Collections.Generic;

    public interface User<THub> where THub : Hub
    {
        string UserId { get; set; }
        IDictionary<string, byte[]> Messages { get; set; }
    }
}
