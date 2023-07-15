namespace MassTransit.Middleware
{
    using System.Threading.Tasks;
    using Transports;


    public class SetRoutingKeyFilter<TMessage> :
        IFilter<SendContext<TMessage>>
        where TMessage : class
    {
        readonly IMessageRoutingKeyFormatter<TMessage> _routingKeyFormatter;

        public SetRoutingKeyFilter(IMessageRoutingKeyFormatter<TMessage> routingKeyFormatter)
        {
            _routingKeyFormatter = routingKeyFormatter;
        }

        public Task Send(SendContext<TMessage> context, IPipe<SendContext<TMessage>> next)
        {
            var routingKey = _routingKeyFormatter.FormatRoutingKey(context);

            if (context.TryGetPayload(out RoutingKeySendContext routingKeySendContext))
                routingKeySendContext.RoutingKey = routingKey;

            return next.Send(context);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("setRoutingKey");
        }
    }
}
