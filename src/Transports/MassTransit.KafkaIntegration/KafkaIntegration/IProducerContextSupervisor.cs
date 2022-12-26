namespace MassTransit.KafkaIntegration
{
    using Transports;


    public interface IProducerContextSupervisor<TKey, TValue> :
        ITransportSupervisor<ProducerContext>,
        IKafkaProducerFactory<TKey, TValue>
        where TValue : class
    {
    }
}
