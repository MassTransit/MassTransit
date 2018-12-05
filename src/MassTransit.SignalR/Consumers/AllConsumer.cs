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

    public class AllConsumer<THub> : IConsumer<All<THub>> where THub : Hub
    {
        static readonly ILog _logger = Logger.Get<AllConsumer<THub>>();

        private readonly MassTransitHubLifetimeManager<THub> _hubLifetimeManager;

        public AllConsumer(HubLifetimeManager<THub> hubLifetimeManager)
        {
            _hubLifetimeManager = hubLifetimeManager as MassTransitHubLifetimeManager<THub> ?? throw new ArgumentNullException(nameof(hubLifetimeManager), "HubLifetimeManager<> must be of type MassTransitHubLifetimeManager<>");
        }

        public async Task Consume(ConsumeContext<All<THub>> context)
        {
            var message = new Lazy<SerializedHubMessage>(() => context.Message.Messages.ToSerializedHubMessage());

            var tasks = new List<Task>(_hubLifetimeManager.Connections.Count);

            foreach (var connection in _hubLifetimeManager.Connections)
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
            catch(Exception e)
            {
                _logger.Warn("Failed writing message.", e);
            }
        }
    }
}
