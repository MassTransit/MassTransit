namespace MassTransit.AzureServiceBusTransport
{
    public interface IPartitionKeyFormatter
    {
        string FormatPartitionKey<T>(SendContext<T> context)
            where T : class;
    }
}
