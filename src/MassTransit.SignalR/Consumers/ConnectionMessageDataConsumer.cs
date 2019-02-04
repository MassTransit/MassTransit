namespace MassTransit.SignalR.Consumers
{
    using MassTransit.SignalR.Contracts;
    using MassTransit.SignalR.Utils;
    using Microsoft.AspNetCore.SignalR;
    using System.Threading.Tasks;

    public class ConnectionMessageDataConsumer<THub> : ConnectionBaseConsumer<THub>, IConsumer<ConnectionMessageData<THub>>
        where THub : Hub
    {
        public ConnectionMessageDataConsumer(HubLifetimeManager<THub> hubLifetimeManager)
            : base(hubLifetimeManager)
        {
        }

        public async Task Consume(ConsumeContext<ConnectionMessageData<THub>> context)
        {
            var messages = await context.Message.Messages.ToProtocolDictionary();
            await Handle(context.Message.ConnectionId, messages);
        }
    }
}
