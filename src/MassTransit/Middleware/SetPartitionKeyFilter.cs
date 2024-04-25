namespace MassTransit.Middleware
{
    using System.Threading.Tasks;
    using Transports;


    public class SetPartitionKeyFilter<TMessage> :
        IFilter<SendContext<TMessage>>
        where TMessage : class
    {
        readonly IMessagePartitionKeyFormatter<TMessage> _routingKeyFormatter;

        public SetPartitionKeyFilter(IMessagePartitionKeyFormatter<TMessage> routingKeyFormatter)
        {
            _routingKeyFormatter = routingKeyFormatter;
        }

        public Task Send(SendContext<TMessage> context, IPipe<SendContext<TMessage>> next)
        {
            var routingKey = _routingKeyFormatter.FormatPartitionKey(context);

            if (context.TryGetPayload(out PartitionKeySendContext routingKeySendContext))
                routingKeySendContext.PartitionKey = routingKey;

            return next.Send(context);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("setPartitionKey");
        }
    }
}
