namespace MassTransit.AzureServiceBusTransport.Middleware
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Context;
    using Scheduling;


    /// <summary>
    /// Adds the service bus message scheduler filter
    /// </summary>
    public class ServiceBusMessageSchedulerFilter :
        IFilter<ConsumeContext>
    {
        void IProbeSite.Probe(ProbeContext context)
        {
            context.CreateFilterScope("serviceBusScheduler");
        }

        [DebuggerNonUserCode]
        Task IFilter<ConsumeContext>.Send(ConsumeContext context, IPipe<ConsumeContext> next)
        {
            context.GetOrAddPayload<MessageSchedulerContext>(() => new ConsumeMessageSchedulerContext(context,
                x => new MessageScheduler(new ServiceBusScheduleMessageProvider(x), x.GetPayload<IBusTopology>()), context.ReceiveContext.InputAddress));

            return next.Send(context);
        }
    }
}
