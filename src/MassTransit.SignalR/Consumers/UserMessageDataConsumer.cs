namespace MassTransit.SignalR.Consumers
{
    using System.Threading.Tasks;
    using Contracts;
    using Microsoft.AspNetCore.SignalR;
    using Utils;


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
