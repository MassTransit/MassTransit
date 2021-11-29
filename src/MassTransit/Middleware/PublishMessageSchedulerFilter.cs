namespace MassTransit.Middleware
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Context;
    using Scheduling;


    /// <summary>
    /// Adds the scheduler to the consume context, so that it can be used for message redelivery
    /// </summary>
    public class PublishMessageSchedulerFilter :
        IFilter<ConsumeContext>
    {
        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("scheduler");
            scope.Add("type", "publish");
        }

        [DebuggerNonUserCode]
        Task IFilter<ConsumeContext>.Send(ConsumeContext context, IPipe<ConsumeContext> next)
        {
            MessageSchedulerContext PayloadFactory()
            {
                IMessageScheduler Factory()
                {
                    return new MessageScheduler(new PublishScheduleMessageProvider(context), context.GetPayload<IBusTopology>());
                }

                return new ConsumeMessageSchedulerContext(Factory, context.ReceiveContext.InputAddress);
            }

            context.GetOrAddPayload(PayloadFactory);

            return next.Send(context);
        }
    }
}
