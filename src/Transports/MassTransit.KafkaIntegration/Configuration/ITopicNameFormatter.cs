namespace MassTransit.KafkaIntegration.Configuration
{
    public interface ITopicNameFormatter
    {
        string GetTopicName<TKey, TValue>()
            where TValue : class;
    }
}
