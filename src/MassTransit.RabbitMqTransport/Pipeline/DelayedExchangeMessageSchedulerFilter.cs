namespace MassTransit.RabbitMqTransport.Pipeline
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using MassTransit.Scheduling;
    using Scheduling;


    public class DelayedExchangeMessageSchedulerFilter :
        IFilter<ConsumeContext>
    {
        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("scheduler");
            scope.Add("type", "delayed-exchange");
        }

        [DebuggerNonUserCode]
        Task IFilter<ConsumeContext>.Send(ConsumeContext context, IPipe<ConsumeContext> next)
        {
            MessageSchedulerContext PayloadFactory()
            {
                var modelContext = context.ReceiveContext.GetPayload<ModelContext>();

                var scheduler = new MessageScheduler(new DelayedExchangeScheduleMessageProvider(context, modelContext.ConnectionContext.Topology));

                return new ConsumeMessageSchedulerContext(scheduler, context.ReceiveContext.InputAddress);
            }

            context.GetOrAddPayload(PayloadFactory);

            return next.Send(context);
        }
    }
}
