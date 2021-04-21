namespace MassTransit.Pipeline.Filters
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using Scheduling;
    using Topology;
    using Transports.Scheduling;


    public class DelayedMessageSchedulerFilter :
        IFilter<ConsumeContext>
    {
        void IProbeSite.Probe(ProbeContext context)
        {
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
