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
        Task IFilter<ConsumeContext>.Send(ConsumeContext context, IPipe<ConsumeContext> next)
        {
            context.GetOrAddPayload<MessageSchedulerContext>(() => new ConsumeMessageSchedulerContext(context,
                x => new MessageScheduler(new DelayedScheduleMessageProvider(x), x.GetPayload<IBusTopology>()), context.ReceiveContext.InputAddress));

            return next.Send(context);
        }
    }
}
