namespace MassTransit.KafkaIntegration
{
    public interface IKafkaProducerFactory
    {
    }


    public interface IKafkaProducerFactory<TKey, TValue> :
        IKafkaProducerFactory
        where TValue : class
    {
        ITopicProducer<TKey, TValue> CreateProducer();
    }
}
