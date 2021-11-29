namespace MassTransit.AzureServiceBusTransport.Configuration
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Configuration;
    using Middleware;


    public class SetSessionIdMessageSendTopology<T> :
        IMessageSendTopology<T>
        where T : class
    {
        readonly IFilter<SendContext<T>> _filter;

        public SetSessionIdMessageSendTopology(IMessageSessionIdFormatter<T> sessionIdFormatter)
        {
            if (sessionIdFormatter == null)
                throw new ArgumentNullException(nameof(sessionIdFormatter));

            _filter = new Proxy(new SetSessionIdFilter<T>(sessionIdFormatter));
        }

        public void Apply(ITopologyPipeBuilder<SendContext<T>> builder)
        {
            builder.AddFilter(_filter);
        }


        class Proxy :
            IFilter<SendContext<T>>
        {
            readonly IFilter<ServiceBusSendContext<T>> _filter;

            public Proxy(IFilter<ServiceBusSendContext<T>> filter)
            {
                _filter = filter;
            }

            public Task Send(SendContext<T> context, IPipe<SendContext<T>> next)
            {
                var sendContext = context.GetPayload<ServiceBusSendContext<T>>();

                return _filter.Send(sendContext, next);
            }

            public void Probe(ProbeContext context)
            {
            }
        }
    }
}
