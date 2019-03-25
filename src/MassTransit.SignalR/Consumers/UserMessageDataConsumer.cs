namespace MassTransit.SignalR.Consumers
{
    using MassTransit.SignalR.Contracts;
    using MassTransit.SignalR.Utils;
    using Microsoft.AspNetCore.SignalR;
    using System.Threading.Tasks;

    public class UserMessageDataConsumer<THub> : UserBaseConsumer<THub>, IConsumer<UserMessageData<THub>>
        where THub : Hub
    {
        public UserMessageDataConsumer(HubLifetimeManager<THub> hubLifetimeManager)
            : base(hubLifetimeManager)
        {
        }

        public async Task Consume(ConsumeContext<UserMessageData<THub>> context)
        {
            var messages = await context.Message.Messages.ToProtocolDictionary();
            await Handle(context.Message.UserId, messages);
        }
    }
}
