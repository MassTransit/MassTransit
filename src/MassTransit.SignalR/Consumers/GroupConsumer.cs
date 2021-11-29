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


    public class GroupConsumer<THub> :
        IConsumer<Group<THub>>
        where THub : Hub
    {
        readonly MassTransitHubLifetimeManager<THub> _hubLifetimeManager;

        public GroupConsumer(MassTransitHubLifetimeManager<THub> hubLifetimeManager)
        {
            _hubLifetimeManager = hubLifetimeManager;
        }

        public Task Consume(ConsumeContext<Group<THub>> context)
        {
            return Handle(context.Message.GroupName, context.Message.ExcludedConnectionIds, context.Message.Messages);
        }

        async Task Handle(string groupName, string[] excludedConnectionIds, IReadOnlyDictionary<string, byte[]> messages)
        {
            var message = new Lazy<SerializedHubMessage>(messages.ToSerializedHubMessage);

            var groupStore = _hubLifetimeManager.Groups[groupName];

            if (groupStore == null || groupStore.Count <= 0)
                return;

            var tasks = new List<Task>();
            foreach (var connection in groupStore)
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
