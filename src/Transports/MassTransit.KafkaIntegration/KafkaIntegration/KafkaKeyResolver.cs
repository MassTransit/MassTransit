namespace MassTransit.KafkaIntegration
{
    public delegate TKey KafkaKeyResolver<out TKey, TValue>(KafkaSendContext<TValue> context)
        where TValue : class;
}
