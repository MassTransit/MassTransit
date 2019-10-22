namespace MassTransit.SignalR.Consumers
{
    using Utils;
    using Microsoft.AspNetCore.SignalR;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Context;


    public class GroupBaseConsumer<THub>
        where THub : Hub
    {
        readonly BaseMassTransitHubLifetimeManager<THub> _hubLifetimeManager;

        protected GroupBaseConsumer(HubLifetimeManager<THub> hubLifetimeManager)
        {
            _hubLifetimeManager = hubLifetimeManager as BaseMassTransitHubLifetimeManager<THub> ?? throw new ArgumentNullException(nameof(hubLifetimeManager),
                "HubLifetimeManager<> must be of type BaseMassTransitHubLifetimeManager<>");
        }

        protected async Task Handle(string groupName, string[] excludedConnectionIds, IDictionary<string, byte[]> messages)
        {
            var message = new Lazy<SerializedHubMessage>(messages.ToSerializedHubMessage);

            var groupStore = _hubLifetimeManager.Groups[groupName];

            if (groupStore == null || groupStore.Count <= 0)
                return;

            var tasks = new List<Task>();
            foreach (var connection in groupStore)
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
