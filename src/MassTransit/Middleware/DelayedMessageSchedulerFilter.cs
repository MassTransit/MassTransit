namespace MassTransit.Middleware
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Context;
    using Scheduling;


    public class DelayedMessageSchedulerFilter :
        IFilter<ConsumeContext>
    {
        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("delayedMessageScheduler");
        }

        [DebuggerNonUserCode]
        public Task Send(ConsumeContext context, IPipe<ConsumeContext> next)
        {
            context.GetOrAddPayload<MessageSchedulerContext>(() => new ConsumeMessageSchedulerContext(context, SchedulerFactory));

            return next.Send(context);
        }

        static IMessageScheduler SchedulerFactory(ConsumeContext context)
        {
            return new MessageScheduler(new DelayedScheduleMessageProvider(context), context.GetPayload<IBusTopology>());
        }
    }
}
