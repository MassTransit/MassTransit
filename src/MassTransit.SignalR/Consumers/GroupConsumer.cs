namespace MassTransit.SignalR.Consumers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Contracts;
    using Logging;
    using Microsoft.AspNetCore.SignalR;
    using Utils;


    public class GroupConsumer<THub> :
        IConsumer<Group<THub>>
        where THub : Hub
    {
        static readonly ILog _logger = Logger.Get<GroupConsumer<THub>>();

        readonly MassTransitHubLifetimeManager<THub> _hubLifetimeManager;

        public GroupConsumer(HubLifetimeManager<THub> hubLifetimeManager)
        {
            _hubLifetimeManager = hubLifetimeManager as MassTransitHubLifetimeManager<THub> ?? throw new ArgumentNullException(nameof(hubLifetimeManager),
                "HubLifetimeManager<> must be of type MassTransitHubLifetimeManager<>");
        }

        public async Task Consume(ConsumeContext<Group<THub>> context)
        {
            var message = new Lazy<SerializedHubMessage>(() => context.Message.Messages.ToSerializedHubMessage());

            var groupStore = _hubLifetimeManager.Groups[context.Message.GroupName];

            if (groupStore == null || groupStore.Count <= 0)
                return;

            var tasks = new List<Task>();
            foreach (var connection in groupStore)
            {
                if (context.Message.ExcludedConnectionIds == null
                    || !context.Message.ExcludedConnectionIds.Contains(connection.ConnectionId, StringComparer.OrdinalIgnoreCase))
                {
                    tasks.Add(connection.WriteAsync(message.Value).AsTask());
                }
            }

            try
            {
                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.Warn("Failed writing message.", e);
            }
        }
    }
}
