namespace MassTransit.EventStoreDbIntegration
{
    public interface EventStoreDbSendContext :
        SendContext
    {
        string StreamName { get; set; }
        string EventStoreDBContentType { get; }
    }

    public interface EventStoreDbSendContext<out T> :
        SendContext<T>,
        EventStoreDbSendContext
        where T : class
    {
    }
}
