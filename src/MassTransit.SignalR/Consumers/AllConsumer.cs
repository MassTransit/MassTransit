namespace MassTransit.SignalR.Consumers
{
    using Contracts;
    using Microsoft.AspNetCore.SignalR;
    using System.Threading.Tasks;

    public class AllConsumer<THub> :
	    AllBaseConsumer<THub>,
	    IConsumer<All<THub>>
		where THub : Hub
    {
        public AllConsumer(HubLifetimeManager<THub> hubLifetimeManager)
            : base(hubLifetimeManager)
        {
        }

        public Task Consume(ConsumeContext<All<THub>> context)
        {
            return Handle(context.Message.ExcludedConnectionIds, context.Message.Messages);
        }
    }
}
