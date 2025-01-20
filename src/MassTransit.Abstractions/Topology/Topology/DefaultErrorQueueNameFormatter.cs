namespace MassTransit.Topology
{
    public class DefaultErrorQueueNameFormatter :
        IErrorQueueNameFormatter
    {
        const string ErrorQueueSuffix = "_error";

        public static readonly IErrorQueueNameFormatter Instance = new DefaultErrorQueueNameFormatter();

        public string FormatErrorQueueName(string queueName)
        {
            return queueName + ErrorQueueSuffix;
        }
    }
}
