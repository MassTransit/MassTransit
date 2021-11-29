namespace MassTransit.SignalR.Consumers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Context;
    using Contracts;
    using Logging;
    using Microsoft.AspNetCore.SignalR;
    using Utils;


    public class UserConsumer<THub> :
        IConsumer<User<THub>>
        where THub : Hub
    {
        readonly MassTransitHubLifetimeManager<THub> _hubLifetimeManager;

        public UserConsumer(MassTransitHubLifetimeManager<THub> hubLifetimeManager)
        {
            _hubLifetimeManager = hubLifetimeManager;
        }

        public Task Consume(ConsumeContext<User<THub>> context)
        {
            return Handle(context.Message.UserId, context.Message.Messages);
        }

        async Task Handle(string userId, IReadOnlyDictionary<string, byte[]> messages)
        {
            var message = new Lazy<SerializedHubMessage>(messages.ToSerializedHubMessage);

            var userStore = _hubLifetimeManager.Users[userId];

            if (userStore == null || userStore.Count <= 0)
                return;

            var tasks = new List<Task>(userStore.Count);
            foreach (var connection in userStore)
                tasks.Add(connection.WriteAsync(message.Value).AsTask());

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
