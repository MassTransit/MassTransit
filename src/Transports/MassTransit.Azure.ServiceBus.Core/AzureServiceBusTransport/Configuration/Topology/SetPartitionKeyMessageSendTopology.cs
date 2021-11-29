namespace MassTransit.AzureServiceBusTransport.Configuration
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Configuration;
    using Middleware;


    public class SetPartitionKeyMessageSendTopology<T> :
        IMessageSendTopology<T>
        where T : class
    {
        readonly IFilter<SendContext<T>> _filter;

        public SetPartitionKeyMessageSendTopology(IMessagePartitionKeyFormatter<T> partitionKeyFormatter)
        {
            if (partitionKeyFormatter == null)
                throw new ArgumentNullException(nameof(partitionKeyFormatter));

            _filter = new Proxy(new SetPartitionKeyFilter<T>(partitionKeyFormatter));
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
                var serviceBusSendContext = context.GetPayload<ServiceBusSendContext<T>>();

                return _filter.Send(serviceBusSendContext, next);
            }

            public void Probe(ProbeContext context)
            {
            }
        }
    }
}
