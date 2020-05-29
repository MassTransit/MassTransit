namespace MassTransit.Azure.ServiceBus.Core.Topology.Conventions
{
    public interface IMessagePartitionKeyFormatter<in TMessage>
        where TMessage : class
    {
        string FormatPartitionKey(SendContext<TMessage> context);
    }
}
