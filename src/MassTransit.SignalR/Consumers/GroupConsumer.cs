namespace MassTransit.SignalR.Consumers
{
    using System.Threading.Tasks;
    using Contracts;
    using Microsoft.AspNetCore.SignalR;
    public class GroupConsumer<THub> :
	    GroupBaseConsumer<THub>,
        IConsumer<Group<THub>>
        where THub : Hub
    {
        public GroupConsumer(HubLifetimeManager<THub> hubLifetimeManager)
            : base(hubLifetimeManager)
        {
        }

        public Task Consume(ConsumeContext<Group<THub>> context)
        {
            return Handle(context.Message.GroupName, context.Message.ExcludedConnectionIds, context.Message.Messages);
        }
    }
}
