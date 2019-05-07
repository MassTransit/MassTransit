namespace MassTransit.SignalR.Consumers
{
    using MassTransit.Logging;
    using MassTransit.SignalR.Utils;
    using Microsoft.AspNetCore.SignalR;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;


    public class ConnectionBaseConsumer<THub>
        where THub : Hub
    {
        static readonly ILogger _logger = Logger.Get<ConnectionBaseConsumer<THub>>();

        private readonly BaseMassTransitHubLifetimeManager<THub> _hubLifetimeManager;

        protected ConnectionBaseConsumer(HubLifetimeManager<THub> hubLifetimeManager)
        {
            _hubLifetimeManager = hubLifetimeManager as BaseMassTransitHubLifetimeManager<THub> ?? throw new ArgumentNullException(nameof(hubLifetimeManager),
                "HubLifetimeManager<> must be of type BaseMassTransitHubLifetimeManager<>");
        }

        protected async Task Handle(string connectionId, IDictionary<string, byte[]> messages)
        {
            var message = new Lazy<SerializedHubMessage>(() => messages.ToSerializedHubMessage());

            var connection = _hubLifetimeManager.Connections[connectionId];
            if (connection == null)
                return; // Connection doesn't exist on server, skipping

            try
            {
                await connection.WriteAsync(message.Value).AsTask();
            }
            catch (Exception e)
            {
                _logger.LogWarning("Failed writing message.", e);
            }
        }
    }
}
