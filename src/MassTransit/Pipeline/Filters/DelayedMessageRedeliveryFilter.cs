namespace MassTransit.Pipeline.Filters
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using GreenPipes;
    using Transports.Scheduling;


    /// <summary>
    /// Uses a delayed exchange in ActiveMQ to delay a message retry
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public class DelayedMessageRedeliveryFilter<TMessage> :
        IFilter<ConsumeContext<TMessage>>
        where TMessage : class
    {
        void IProbeSite.Probe(ProbeContext context)
        {
        }

        [DebuggerNonUserCode]
        Task IFilter<ConsumeContext<TMessage>>.Send(ConsumeContext<TMessage> context, IPipe<ConsumeContext<TMessage>> next)
        {
            context.GetOrAddPayload<MessageRedeliveryContext>(() => new DelayedMessageRedeliveryContext<TMessage>(context));

            return next.Send(context);
        }
    }
}
