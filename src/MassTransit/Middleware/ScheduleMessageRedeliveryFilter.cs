namespace MassTransit.Middleware
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Context;


    /// <summary>
    /// Adds the scheduler to the consume context, so that it can be used for message redelivery
    /// </summary>
    public class ScheduleMessageRedeliveryFilter<TMessage> :
        IFilter<ConsumeContext<TMessage>>
        where TMessage : class
    {
        readonly RedeliveryOptions _options;

        public ScheduleMessageRedeliveryFilter(RedeliveryOptions options)
        {
            _options = options;
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("scheduleRedeliveryContext");
        }

        [DebuggerNonUserCode]
        public Task Send(ConsumeContext<TMessage> context, IPipe<ConsumeContext<TMessage>> next)
        {
            context.GetOrAddPayload<MessageRedeliveryContext>(() => new ScheduleMessageRedeliveryContext<TMessage>(context, _options));

            return next.Send(context);
        }
    }
}
