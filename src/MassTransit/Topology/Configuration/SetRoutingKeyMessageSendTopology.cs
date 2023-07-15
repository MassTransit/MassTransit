namespace MassTransit.Configuration
{
    using System;
    using Middleware;
    using Transports;


    public class SetRoutingKeyMessageSendTopology<TMessage> :
        IMessageSendTopology<TMessage>
        where TMessage : class
    {
        readonly IFilter<SendContext<TMessage>> _filter;

        public SetRoutingKeyMessageSendTopology(IMessageRoutingKeyFormatter<TMessage> routingKeyFormatter)
        {
            if (routingKeyFormatter == null)
                throw new ArgumentNullException(nameof(routingKeyFormatter));

            _filter = new SetRoutingKeyFilter<TMessage>(routingKeyFormatter);
        }

        public void Apply(ITopologyPipeBuilder<SendContext<TMessage>> builder)
        {
            builder.AddFilter(_filter);
        }
    }
}
