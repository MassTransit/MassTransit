namespace MassTransit.SignalR.Consumers
{
    using MassTransit.Logging;
    using MassTransit.SignalR.Contracts;
    using MassTransit.SignalR.Utils;
    using Microsoft.AspNetCore.SignalR;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class GroupConsumer<THub> : IConsumer<Group<THub>> where THub : Hub
    {
        static readonly ILog _logger = Logger.Get<GroupConsumer<THub>>();

        private readonly MassTransitHubLifetimeManager<THub> _hubLifetimeManager;

        public GroupConsumer(HubLifetimeManager<THub> hubLifetimeManager)
        {
            _hubLifetimeManager = hubLifetimeManager as MassTransitHubLifetimeManager<THub> ?? throw new ArgumentNullException(nameof(hubLifetimeManager), "HubLifetimeManager<> must be of type MassTransitHubLifetimeManager<>");
        }

        public async Task Consume(ConsumeContext<Group<THub>> context)
        {
            var message = new Lazy<SerializedHubMessage>(() => context.Message.Messages.ToSerializedHubMessage());

            var groupStore = _hubLifetimeManager.Groups[context.Message.GroupName];

            if (groupStore == null || groupStore.Count <= 0) return;

            var tasks = new List<Task>();
            foreach (var connection in groupStore)
            {
                if (context.Message.ExcludedConnectionIds == null || !context.Message.ExcludedConnectionIds.Contains(connection.ConnectionId, StringComparer.OrdinalIgnoreCase))
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
                _logger.Warn("Failed writing message.", e);
            }
        }
    }
}
