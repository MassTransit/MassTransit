namespace MassTransit
{
    public interface EventHubSendContext :
        SendContext,
        PartitionKeySendContext
    {
        string PartitionId { get; set; }
    }


    public interface EventHubSendContext<out T> :
        SendContext<T>,
        EventHubSendContext
        where T : class
    {
    }
}
