namespace MassTransit.SignalR.Consumers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Context;
    using Contracts;
    using Logging;
    using Microsoft.AspNetCore.SignalR;
    using Utils;


    public class AllConsumer<THub> :
        IConsumer<All<THub>>
        where THub : Hub
    {
        readonly MassTransitHubLifetimeManager<THub> _hubLifetimeManager;

        public AllConsumer(MassTransitHubLifetimeManager<THub> hubLifetimeManager)
        {
            _hubLifetimeManager = hubLifetimeManager;
        }

        public Task Consume(ConsumeContext<All<THub>> context)
        {
            return Handle(context.Message.ExcludedConnectionIds, context.Message.Messages);
        }

        async Task Handle(string[] excludedConnectionIds, IReadOnlyDictionary<string, byte[]> messages)
        {
            var message = new Lazy<SerializedHubMessage>(messages.ToSerializedHubMessage);

            var tasks = new List<Task>(_hubLifetimeManager.Connections.Count);

            foreach (var connection in _hubLifetimeManager.Connections)
            {
                if (excludedConnectionIds == null || !excludedConnectionIds.Contains(connection.ConnectionId, StringComparer.OrdinalIgnoreCase))
                    tasks.Add(connection.WriteAsync(message.Value).AsTask());
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
