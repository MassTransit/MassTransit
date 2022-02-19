namespace MassTransit.GrpcTransport.Configuration
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Configuration;
    using Middleware;


    public class SetRoutingKeyMessageSendTopology<T> :
        IMessageSendTopology<T>
        where T : class
    {
        readonly IFilter<SendContext<T>> _filter;

        public SetRoutingKeyMessageSendTopology(IMessageRoutingKeyFormatter<T> routingKeyFormatter)
        {
            if (routingKeyFormatter == null)
                throw new ArgumentNullException(nameof(routingKeyFormatter));

            _filter = new Proxy(new SetRoutingKeyFilter<T>(routingKeyFormatter));
        }

        public void Apply(ITopologyPipeBuilder<SendContext<T>> builder)
        {
            builder.AddFilter(_filter);
        }


        class Proxy :
            IFilter<SendContext<T>>
        {
            readonly IFilter<GrpcSendContext<T>> _filter;

            public Proxy(IFilter<GrpcSendContext<T>> filter)
            {
                _filter = filter;
            }

            public Task Send(SendContext<T> context, IPipe<SendContext<T>> next)
            {
                var rabbitMqSendContext = context.GetPayload<GrpcSendContext<T>>();

                return _filter.Send(rabbitMqSendContext, next);
            }

            public void Probe(ProbeContext context)
            {
            }
        }
    }
}
