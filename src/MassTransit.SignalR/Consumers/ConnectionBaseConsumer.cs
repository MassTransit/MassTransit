namespace MassTransit.SignalR.Consumers
{
    using Utils;
    using Microsoft.AspNetCore.SignalR;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Context;


    public class ConnectionBaseConsumer<THub>
        where THub : Hub
    {
        readonly BaseMassTransitHubLifetimeManager<THub> _hubLifetimeManager;

        protected ConnectionBaseConsumer(HubLifetimeManager<THub> hubLifetimeManager)
        {
            _hubLifetimeManager = hubLifetimeManager as BaseMassTransitHubLifetimeManager<THub> ?? throw new ArgumentNullException(nameof(hubLifetimeManager), "HubLifetimeManager<> must be of type BaseMassTransitHubLifetimeManager<>");
        }

        protected async Task Handle(string connectionId, IDictionary<string, byte[]> messages)
        {
            var message = new Lazy<SerializedHubMessage>(messages.ToSerializedHubMessage);

            var connection = _hubLifetimeManager.Connections[connectionId];
            if (connection == null) return; // Connection doesn't exist on server, skipping

            try
            {
                await connection.WriteAsync(message.Value).AsTask();
            }
            catch (Exception e)
            {
                LogContext.Warning?.Log(e, "Failed to write message");
            }
        }
    }
}
