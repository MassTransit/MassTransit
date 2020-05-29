namespace MassTransit.Azure.ServiceBus.Core.Topology.Conventions.PartitionKey
{
    public class EmptyPartitionKeyFormatter :
        IPartitionKeyFormatter
    {
        string IPartitionKeyFormatter.FormatPartitionKey<T>(SendContext<T> context)
        {
            return null;
        }
    }
}
