namespace MassTransit.Middleware
{
    using System.Diagnostics;
    using System.Threading.Tasks;


    /// <summary>
    /// Uses a delayed exchange in ActiveMQ to delay a message retry
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public class DelayedMessageRedeliveryFilter<TMessage> :
        IFilter<ConsumeContext<TMessage>>
        where TMessage : class
    {
        readonly RedeliveryOptions _options;

        public DelayedMessageRedeliveryFilter(RedeliveryOptions options)
        {
            _options = options;
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("delayedMessageRedelivery");
            scope.Add("messageType", TypeCache<TMessage>.ShortName);
        }

        [DebuggerNonUserCode]
        public Task Send(ConsumeContext<TMessage> context, IPipe<ConsumeContext<TMessage>> next)
        {
            context.GetOrAddPayload<MessageRedeliveryContext>(() => new DelayedMessageRedeliveryContext<TMessage>(context, _options));

            return next.Send(context);
        }
    }
}
