namespace MassTransit.Conductor.Consumers
{
    using System.Threading.Tasks;
    using Client;
    using Context;
    using Contracts;
    using Util;


    public class InstanceDownConsumer :
        IConsumer<InstanceDown>
    {
        readonly IServiceInstanceCache _instanceCache;

        public InstanceDownConsumer(IServiceInstanceCache instanceCache)
        {
            _instanceCache = instanceCache;
        }

        public Task Consume(ConsumeContext<InstanceDown> context)
        {
            LogContext.Debug?.Log("Down: {ServiceAddress} {InstanceId}", context.Message.Service.ServiceAddress, context.Message.Instance.InstanceId);

            _instanceCache.Remove(context.Message.Instance.InstanceId);

            return TaskUtil.Completed;
        }
    }
}
