namespace MassTransit.Configuration;

using System;
using Middleware;
using Transports;


public class SetPartitionKeyMessageSendTopology<TMessage> :
    IMessageSendTopology<TMessage>
    where TMessage : class
{
    readonly IFilter<SendContext<TMessage>> _filter;

    public SetPartitionKeyMessageSendTopology(IMessagePartitionKeyFormatter<TMessage> partitionKeyFormatter)
    {
        if (partitionKeyFormatter == null)
            throw new ArgumentNullException(nameof(partitionKeyFormatter));

        _filter = new SetPartitionKeyFilter<TMessage>(partitionKeyFormatter);
    }

    public void Apply(ITopologyPipeBuilder<SendContext<TMessage>> builder)
    {
        builder.AddFilter(_filter);
    }
}
