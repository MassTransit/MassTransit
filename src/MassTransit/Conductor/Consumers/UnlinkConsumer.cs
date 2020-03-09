namespace MassTransit.Conductor.Consumers
{
    using System.Threading.Tasks;
    using Context;
    using Contracts;
    using Server;
    using Util;


    public class UnlinkConsumer<T> :
        IConsumer<Unlink<T>>
        where T : class
    {
        readonly IServiceEndpointMessageClientCache _clientCache;

        public UnlinkConsumer(IServiceEndpointMessageClientCache clientCache)
        {
            _clientCache = clientCache;
        }

        public Task Consume(ConsumeContext<Unlink<T>> context)
        {
            LogContext.Debug?.Log("Unlinking Client: {Id}", context.Message.ClientId);

            _clientCache.Unlink(context.Message.ClientId);

            return TaskUtil.Completed;
        }
    }
}
