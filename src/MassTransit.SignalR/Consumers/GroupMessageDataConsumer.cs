namespace MassTransit.SignalR.Consumers
{
    using System.Threading.Tasks;
    using Contracts;
    using Microsoft.AspNetCore.SignalR;
    using Utils;


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
