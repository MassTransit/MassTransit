namespace MassTransit.SignalR.Contracts
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.SignalR;


    public interface User<THub>
        where THub : Hub
    {
        string UserId { get; }
        IReadOnlyDictionary<string, byte[]> Messages { get; }
    }
}
