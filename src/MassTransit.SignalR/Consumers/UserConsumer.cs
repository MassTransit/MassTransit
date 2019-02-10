namespace MassTransit.SignalR.Consumers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Contracts;
    using Logging;
    using Microsoft.AspNetCore.SignalR;
    using Utils;


    public class UserConsumer<THub> :
        IConsumer<User<THub>>
        where THub : Hub
    {
        static readonly ILog _logger = Logger.Get<UserConsumer<THub>>();

        readonly MassTransitHubLifetimeManager<THub> _hubLifetimeManager;

        public UserConsumer(HubLifetimeManager<THub> hubLifetimeManager)
        {
            _hubLifetimeManager = hubLifetimeManager as MassTransitHubLifetimeManager<THub> ?? throw new ArgumentNullException(nameof(hubLifetimeManager),
                "HubLifetimeManager<> must be of type MassTransitHubLifetimeManager<>");
        }

        public async Task Consume(ConsumeContext<User<THub>> context)
        {
            var message = new Lazy<SerializedHubMessage>(() => context.Message.Messages.ToSerializedHubMessage());

            var userStore = _hubLifetimeManager.Users[context.Message.UserId];

            if (userStore == null || userStore.Count <= 0)
                return;

            var tasks = new List<Task>();
            foreach (var connection in userStore)
            {
                tasks.Add(connection.WriteAsync(message.Value).AsTask());
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
