namespace MassTransit.SignalR.Consumers
{
    using Utils;
    using Microsoft.AspNetCore.SignalR;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Context;


    public abstract class AllBaseConsumer<THub>
        where THub : Hub
    {
        readonly BaseMassTransitHubLifetimeManager<THub> _hubLifetimeManager;

        protected AllBaseConsumer(HubLifetimeManager<THub> hubLifetimeManager)
        {
            _hubLifetimeManager = hubLifetimeManager as BaseMassTransitHubLifetimeManager<THub> ?? throw new ArgumentNullException(nameof(hubLifetimeManager), "HubLifetimeManager<> must be of type BaseMassTransitHubLifetimeManager<>");
        }

        protected async Task Handle(string[] excludedConnectionIds, IDictionary<string, byte[]> messages)
        {
            var message = new Lazy<SerializedHubMessage>(messages.ToSerializedHubMessage);

            var tasks = new List<Task>(_hubLifetimeManager.Connections.Count);

            foreach (var connection in _hubLifetimeManager.Connections)
            {
                if (excludedConnectionIds == null || !excludedConnectionIds.Contains(connection.ConnectionId, StringComparer.OrdinalIgnoreCase))
                {
                    tasks.Add(connection.WriteAsync(message.Value).AsTask());
                }
            }

            try
            {
                await Task.WhenAll(tasks);
            }
            catch (Exception e)
            {
                LogContext.Warning?.Log(e, "Failed to write message");
            }
        }
    }
}
