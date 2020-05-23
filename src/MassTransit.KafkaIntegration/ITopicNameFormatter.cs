namespace MassTransit.KafkaIntegration
{
    public interface ITopicNameFormatter
    {
        string GetTopicName<TKey, TValue>()
            where TValue : class;
    }


    public class DefaultTopicNameFormatter :
        ITopicNameFormatter
    {
        readonly string _topic;

        public DefaultTopicNameFormatter(string topic)
        {
            _topic = topic;
        }

        public string GetTopicName<TKey, TValue>()
            where TValue : class
        {
            return _topic;
        }
    }
}
