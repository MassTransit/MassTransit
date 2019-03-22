namespace MassTransit.SignalR.Consumers
{
    using System.Threading.Tasks;
    using Contracts;
    using Microsoft.AspNetCore.SignalR;
    public class UserConsumer<THub> :
	    UserBaseConsumer<THub>,
        IConsumer<User<THub>>
        where THub : Hub
    {
        public UserConsumer(HubLifetimeManager<THub> hubLifetimeManager)
            : base(hubLifetimeManager)
        {
        }

        public Task Consume(ConsumeContext<User<THub>> context)
        {
            return Handle(context.Message.UserId, context.Message.Messages);
        }
    }
}
