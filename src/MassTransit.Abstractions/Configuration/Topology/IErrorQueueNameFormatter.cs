namespace MassTransit
{
    public interface IErrorQueueNameFormatter
    {
        string FormatErrorQueueName(string queueName);
    }
}
