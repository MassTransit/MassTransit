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
            MessageSchedulerContext PayloadFactory()
            {
                IMessageScheduler Factory()
                {
                    return new MessageScheduler(new DelayedScheduleMessageProvider(context), context.GetPayload<IBusTopology>());
                }

                return new ConsumeMessageSchedulerContext(Factory, context.ReceiveContext.InputAddress);
            }

            context.GetOrAddPayload(PayloadFactory);

            return next.Send(context);
        }
    }
}
