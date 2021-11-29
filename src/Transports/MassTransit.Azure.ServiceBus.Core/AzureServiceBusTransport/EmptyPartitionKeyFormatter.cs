namespace MassTransit.AzureServiceBusTransport
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
