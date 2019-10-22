namespace MassTransit.Pipeline.Filters
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;


    /// <summary>
    /// Adds the scheduler to the consume context, so that it can be used for message redelivery
    /// </summary>
    public class ScheduleMessageRedeliveryFilter<TMessage> :
        IFilter<ConsumeContext<TMessage>>
        where TMessage : class
    {
        void IProbeSite.Probe(ProbeContext context)
        {
            context.CreateFilterScope("scheduleRedeliveryContext");
        }

        [DebuggerNonUserCode]
        Task IFilter<ConsumeContext<TMessage>>.Send(ConsumeContext<TMessage> context, IPipe<ConsumeContext<TMessage>> next)
        {
            context.GetOrAddPayload<MessageRedeliveryContext>(() => new ScheduleMessageRedeliveryContext<TMessage>(context));

            return next.Send(context);
        }
    }
}
