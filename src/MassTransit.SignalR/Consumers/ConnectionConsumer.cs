namespace MassTransit.SignalR.Consumers
{
    using MassTransit.Logging;
    using MassTransit.SignalR.Contracts;
    using MassTransit.SignalR.Utils;
    using Microsoft.AspNetCore.SignalR;
    using System;
    using System.Threading.Tasks;

    public class ConnectionConsumer<THub> : IConsumer<Connection<THub>> where THub : Hub
    {
        static readonly ILog _logger = Logger.Get<ConnectionConsumer<THub>>();

        private readonly MassTransitHubLifetimeManager<THub> _hubLifetimeManager;

        public ConnectionConsumer(HubLifetimeManager<THub> hubLifetimeManager)
        {
            _hubLifetimeManager = hubLifetimeManager as MassTransitHubLifetimeManager<THub> ?? throw new ArgumentNullException(nameof(hubLifetimeManager), "HubLifetimeManager<> must be of type MassTransitHubLifetimeManager<>");
        }

        public async Task Consume(ConsumeContext<Connection<THub>> context)
        {
            var message = new Lazy<SerializedHubMessage>(() => context.Message.Messages.ToSerializedHubMessage());

            var connection = _hubLifetimeManager.Connections[context.Message.ConnectionId];
            if (connection == null) return; // Connection doesn't exist on server, skipping

            try
            {
                await connection.WriteAsync(message.Value).AsTask();
            }
            catch (Exception e)
            {
                _logger.Warn("Failed writing message.", e);
            }
        }
    }
}
