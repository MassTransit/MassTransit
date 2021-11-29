namespace MassTransit.RabbitMqTransport.Middleware
{
    using System.Threading.Tasks;


    public class SetRoutingKeyFilter<T> :
        IFilter<RabbitMqSendContext<T>>
        where T : class
    {
        readonly IMessageRoutingKeyFormatter<T> _routingKeyFormatter;

        public SetRoutingKeyFilter(IMessageRoutingKeyFormatter<T> routingKeyFormatter)
        {
            _routingKeyFormatter = routingKeyFormatter;
        }

        public Task Send(RabbitMqSendContext<T> context, IPipe<RabbitMqSendContext<T>> next)
        {
            var routingKey = _routingKeyFormatter.FormatRoutingKey(context);

            context.RoutingKey = routingKey;

            return next.Send(context);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("SetCorrelationId");
        }
    }
}
