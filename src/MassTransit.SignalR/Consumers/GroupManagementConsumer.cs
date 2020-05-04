namespace MassTransit.SignalR.Consumers
{
    using System.Threading.Tasks;
    using Contracts;
    using Microsoft.AspNetCore.SignalR;


    public class GroupManagementConsumer<THub> :
        IConsumer<GroupManagement<THub>>
        where THub : Hub
    {
        readonly MassTransitHubLifetimeManager<THub> _hubLifetimeManager;

        public GroupManagementConsumer(MassTransitHubLifetimeManager<THub> hubLifetimeManager)
        {
            _hubLifetimeManager = hubLifetimeManager;
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
