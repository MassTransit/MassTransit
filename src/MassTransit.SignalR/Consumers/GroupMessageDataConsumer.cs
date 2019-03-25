namespace MassTransit.SignalR.Consumers
{
    using MassTransit.SignalR.Contracts;
    using MassTransit.SignalR.Utils;
    using Microsoft.AspNetCore.SignalR;
    using System.Threading.Tasks;

    public class GroupMessageDataConsumer<THub> : GroupBaseConsumer<THub>, IConsumer<GroupMessageData<THub>>
        where THub : Hub
    {
        public GroupMessageDataConsumer(HubLifetimeManager<THub> hubLifetimeManager)
            : base(hubLifetimeManager)
        {
        }

        public async Task Consume(ConsumeContext<GroupMessageData<THub>> context)
        {
            var messages = await context.Message.Messages.ToProtocolDictionary();
            await Handle(context.Message.GroupName, context.Message.ExcludedConnectionIds, messages);
        }
    }
}
