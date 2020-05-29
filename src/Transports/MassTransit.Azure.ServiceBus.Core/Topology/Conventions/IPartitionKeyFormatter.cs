namespace MassTransit.Azure.ServiceBus.Core.Topology.Conventions
{
    public interface IPartitionKeyFormatter
    {
        string FormatPartitionKey<T>(SendContext<T> context)
            where T : class;
    }
}
