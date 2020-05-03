namespace MassTransit.SignalR.Consumers
{
    using System.Threading.Tasks;
    using Contracts;
    using Microsoft.AspNetCore.SignalR;
    using Utils;


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
