namespace MassTransit.GrpcTransport.Pipeline
{
    using System.Threading.Tasks;
    using GreenPipes;
    using Topology;


    public class SetRoutingKeyFilter<T> :
        IFilter<GrpcSendContext<T>>
        where T : class
    {
        readonly IMessageRoutingKeyFormatter<T> _routingKeyFormatter;

        public SetRoutingKeyFilter(IMessageRoutingKeyFormatter<T> routingKeyFormatter)
        {
            _routingKeyFormatter = routingKeyFormatter;
        }

        public Task Send(GrpcSendContext<T> context, IPipe<GrpcSendContext<T>> next)
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
