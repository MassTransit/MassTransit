namespace MassTransit.SqlTransport
{
    public interface NotificationContext :
        PipeContext
    {
        ConnectHandle ConnectNotificationSink(string queueName, IQueueNotificationListener listener);
    }
}
