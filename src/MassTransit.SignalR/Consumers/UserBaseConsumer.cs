namespace MassTransit.SignalR.Consumers
{
    using MassTransit.Logging;
    using MassTransit.SignalR.Utils;
    using Microsoft.AspNetCore.SignalR;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;


    public class UserBaseConsumer<THub>
        where THub : Hub
    {
        static readonly ILogger _logger = Logger.Get<UserBaseConsumer<THub>>();

        private readonly BaseMassTransitHubLifetimeManager<THub> _hubLifetimeManager;

        protected UserBaseConsumer(HubLifetimeManager<THub> hubLifetimeManager)
        {
            _hubLifetimeManager = hubLifetimeManager as BaseMassTransitHubLifetimeManager<THub> ?? throw new ArgumentNullException(nameof(hubLifetimeManager), "HubLifetimeManager<> must be of type BaseMassTransitHubLifetimeManager<>");
        }

        public async Task Handle(string userId, IDictionary<string, byte[]> messages)
        {
            var message = new Lazy<SerializedHubMessage>(() => messages.ToSerializedHubMessage());

            var userStore = _hubLifetimeManager.Users[userId];

            if (userStore == null || userStore.Count <= 0) return;

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
                _logger.LogWarning("Failed writing message.", e);
            }
        }
    }
}
