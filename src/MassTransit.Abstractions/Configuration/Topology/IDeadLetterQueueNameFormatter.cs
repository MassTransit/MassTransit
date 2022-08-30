namespace MassTransit
{
    public interface IDeadLetterQueueNameFormatter
    {
        string FormatDeadLetterQueueName(string queueName);
    }
}
