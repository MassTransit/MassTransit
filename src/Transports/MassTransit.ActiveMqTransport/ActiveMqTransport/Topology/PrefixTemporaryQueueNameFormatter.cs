namespace MassTransit.ActiveMqTransport.Topology
{
    public class PrefixTemporaryQueueNameFormatter :
        IActiveMqTemporaryQueueNameFormatter
    {
        readonly string _prefix;

        public PrefixTemporaryQueueNameFormatter(string prefix)
        {
            _prefix = prefix;
        }

        public string Format(string queueName)
        {
            return $"{_prefix}{queueName}";
        }
    }
}
