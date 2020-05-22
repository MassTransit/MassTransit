namespace MassTransit.KafkaIntegration
{
    public interface ITopicNameFormatter
    {
        string GetTopicName<TKey, TValue>()
            where TValue : class;
    }
}
