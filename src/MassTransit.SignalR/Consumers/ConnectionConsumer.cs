namespace MassTransit.SignalR.Consumers
{
    using Contracts;
    using Microsoft.AspNetCore.SignalR;
    using System.Threading.Tasks;

    public class ConnectionConsumer<THub> :
	    ConnectionBaseConsumer<THub>,
        IConsumer<Connection<THub>>
        where THub : Hub
    {
        public ConnectionConsumer(HubLifetimeManager<THub> hubLifetimeManager)
            : base(hubLifetimeManager)
        {
        }

        public Task Consume(ConsumeContext<Connection<THub>> context)
        {
            return Handle(context.Message.ConnectionId, context.Message.Messages);
        }
    }
}
