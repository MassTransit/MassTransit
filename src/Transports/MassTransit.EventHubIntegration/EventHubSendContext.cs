namespace MassTransit
{
    public interface EventHubSendContext :
        SendContext
    {
        string PartitionId { get; set; }
        string PartitionKey { get; set; }
    }


    public interface EventHubSendContext<out T> :
        SendContext<T>,
        EventHubSendContext
        where T : class
    {
    }
}
