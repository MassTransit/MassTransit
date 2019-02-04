namespace MassTransit.SignalR.Consumers
{
    using MassTransit.SignalR.Contracts;
    using MassTransit.SignalR.Utils;
    using Microsoft.AspNetCore.SignalR;
    using System.Threading.Tasks;

    public class AllMessageDataConsumer<THub> : AllBaseConsumer<THub>, IConsumer<AllMessageData<THub>>
        where THub : Hub
    {
        public AllMessageDataConsumer(HubLifetimeManager<THub> hubLifetimeManager)
            : base(hubLifetimeManager)
        {
        }

        public async Task Consume(ConsumeContext<AllMessageData<THub>> context)
        {
            var messages = await context.Message.Messages.ToProtocolDictionary();
            await Handle(context.Message.ExcludedConnectionIds, messages);
        }
    }
}
