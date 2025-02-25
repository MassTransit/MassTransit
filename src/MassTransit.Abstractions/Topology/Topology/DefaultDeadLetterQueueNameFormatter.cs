namespace MassTransit.Topology
{
    public class DefaultDeadLetterQueueNameFormatter :
        IDeadLetterQueueNameFormatter
    {
        const string DeadLetterQueueSuffix = "_skipped";

        public static readonly IDeadLetterQueueNameFormatter Instance = new DefaultDeadLetterQueueNameFormatter();

        public string FormatDeadLetterQueueName(string queueName)
        {
            return queueName + DeadLetterQueueSuffix;
        }
    }
}
