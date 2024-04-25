namespace MassTransit.AzureServiceBusTransport.Topology
{
    using System;
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

            _filter = new ServiceBusSendContextFilter<T>(new SetPartitionKeyFilter<T>(partitionKeyFormatter));
        }

        public void Apply(ITopologyPipeBuilder<SendContext<T>> builder)
        {
            builder.AddFilter(_filter);
        }
    }
}
