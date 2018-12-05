namespace MassTransit.SignalR.Consumers
{
    using MassTransit.Logging;
    using MassTransit.SignalR.Contracts;
    using Microsoft.AspNetCore.SignalR;
    using System;
    using System.Threading.Tasks;

    public class GroupManagementConsumer<THub> : IConsumer<GroupManagement<THub>> where THub : Hub
    {
        private readonly MassTransitHubLifetimeManager<THub> _hubLifetimeManager;

        public GroupManagementConsumer(HubLifetimeManager<THub> hubLifetimeManager)
        {
            _hubLifetimeManager = hubLifetimeManager as MassTransitHubLifetimeManager<THub> ?? throw new ArgumentNullException(nameof(hubLifetimeManager), "HubLifetimeManager<> must be of type MassTransitHubLifetimeManager<>");
        }

        public Task Consume(ConsumeContext<GroupManagement<THub>> context)
        {
            var connection = _hubLifetimeManager.Connections[context.Message.ConnectionId];

            if (connection == null) return Task.CompletedTask; // Connection doesn't exist on this server, no need to send Ack back

            if (context.Message.Action == GroupAction.Remove)
            {
                _hubLifetimeManager.RemoveGroupAsyncCore(connection, context.Message.GroupName);
            }
            else if (context.Message.Action == GroupAction.Add)
            {
                _hubLifetimeManager.AddGroupAsyncCore(connection, context.Message.GroupName);
            }

            return context.RespondAsync<Ack<THub>>(new { _hubLifetimeManager.ServerName });
        }
    }
}
