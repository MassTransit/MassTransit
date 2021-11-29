namespace MassTransit.KafkaIntegration
{
    public delegate TKey KafkaKeyResolver<out TKey, in TValue>(KafkaSendContext<TValue> context)
        where TValue : class;
}
