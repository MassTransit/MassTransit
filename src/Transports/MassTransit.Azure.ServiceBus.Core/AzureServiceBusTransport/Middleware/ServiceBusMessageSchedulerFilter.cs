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
            context.GetOrAddPayload<MessageSchedulerContext>(() =>
            {
                IMessageScheduler Factory()
                {
                    return new MessageScheduler(new ServiceBusScheduleMessageProvider(context), context.GetPayload<IBusTopology>());
                }

                return new ConsumeMessageSchedulerContext(Factory, context.ReceiveContext.InputAddress);
            });

            return next.Send(context);
        }
    }
}
