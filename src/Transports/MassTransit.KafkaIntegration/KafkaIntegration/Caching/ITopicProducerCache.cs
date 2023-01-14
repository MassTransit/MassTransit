namespace MassTransit.KafkaIntegration.Caching
{
    using System;
    using System.Threading.Tasks;


    public interface ITopicProducerCache<T>
    {
        Task<ITopicProducer<TKey, TValue>> GetProducer<TKey, TValue>(T key, Func<T, ITopicProducer<TKey, TValue>> factory)
            where TValue : class;
    }
}
