namespace MassTransit.SignalR.Consumers
{
    using Utils;
    using Microsoft.AspNetCore.SignalR;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Context;


    public class UserBaseConsumer<THub>
        where THub : Hub
    {
        readonly BaseMassTransitHubLifetimeManager<THub> _hubLifetimeManager;

        protected UserBaseConsumer(HubLifetimeManager<THub> hubLifetimeManager)
        {
            _hubLifetimeManager = hubLifetimeManager as BaseMassTransitHubLifetimeManager<THub> ?? throw new ArgumentNullException(nameof(hubLifetimeManager),
                "HubLifetimeManager<> must be of type BaseMassTransitHubLifetimeManager<>");
        }

        public async Task Handle(string userId, IDictionary<string, byte[]> messages)
        {
            var message = new Lazy<SerializedHubMessage>(messages.ToSerializedHubMessage);

            var userStore = _hubLifetimeManager.Users[userId];

            if (userStore == null || userStore.Count <= 0)
                return;

            var tasks = new List<Task>();
            foreach (var connection in userStore)
            {
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
