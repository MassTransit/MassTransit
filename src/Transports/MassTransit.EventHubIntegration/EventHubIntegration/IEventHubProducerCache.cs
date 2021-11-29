namespace MassTransit.EventHubIntegration
{
    using System;
    using System.Threading.Tasks;


    public interface IEventHubProducerCache<TKey>
    {
        Task<IEventHubProducer> GetProducer(TKey key, Func<TKey, Task<IEventHubProducer>> factory);
    }
}
