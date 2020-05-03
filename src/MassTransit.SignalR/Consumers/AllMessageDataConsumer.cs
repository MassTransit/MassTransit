namespace MassTransit.SignalR.Consumers
{
    using System.Threading.Tasks;
    using Contracts;
    using Microsoft.AspNetCore.SignalR;
    using Utils;


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
