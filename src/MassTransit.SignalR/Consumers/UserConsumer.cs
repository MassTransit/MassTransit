namespace MassTransit.SignalR.Consumers
{
    using MassTransit.Logging;
    using MassTransit.SignalR.Contracts;
    using MassTransit.SignalR.Utils;
    using Microsoft.AspNetCore.SignalR;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class UserConsumer<THub> : IConsumer<User<THub>> where THub : Hub
    {
        static readonly ILog _logger = Logger.Get<UserConsumer<THub>>();

        private readonly MassTransitHubLifetimeManager<THub> _hubLifetimeManager;

        public UserConsumer(HubLifetimeManager<THub> hubLifetimeManager)
        {
            _hubLifetimeManager = hubLifetimeManager as MassTransitHubLifetimeManager<THub> ?? throw new ArgumentNullException(nameof(hubLifetimeManager), "HubLifetimeManager<> must be of type MassTransitHubLifetimeManager<>");
        }

        public async Task Consume(ConsumeContext<User<THub>> context)
        {
            var message = new Lazy<SerializedHubMessage>(() => context.Message.Messages.ToSerializedHubMessage());

            var userStore = _hubLifetimeManager.Users[context.Message.UserId];

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
                _logger.Warn("Failed writing message.", e);
            }
        }
    }
}
