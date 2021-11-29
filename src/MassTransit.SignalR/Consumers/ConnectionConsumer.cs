namespace MassTransit.SignalR.Consumers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Context;
    using Contracts;
    using Logging;
    using Microsoft.AspNetCore.SignalR;
    using Utils;


    public class ConnectionConsumer<THub> :
        IConsumer<Connection<THub>>
        where THub : Hub
    {
        readonly MassTransitHubLifetimeManager<THub> _hubLifetimeManager;

        public ConnectionConsumer(MassTransitHubLifetimeManager<THub> hubLifetimeManager)
        {
            _hubLifetimeManager = hubLifetimeManager;
        }

        public Task Consume(ConsumeContext<Connection<THub>> context)
        {
            return Handle(context.Message.ConnectionId, context.Message.Messages);
        }

        async Task Handle(string connectionId, IReadOnlyDictionary<string, byte[]> messages)
        {
            var message = new Lazy<SerializedHubMessage>(messages.ToSerializedHubMessage);

            var connection = _hubLifetimeManager.Connections[connectionId];
            if (connection == null)
                return; // Connection doesn't exist on server, skipping

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
