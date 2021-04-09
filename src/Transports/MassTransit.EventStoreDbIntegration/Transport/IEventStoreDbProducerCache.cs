using System;
using System.Threading.Tasks;

namespace MassTransit.EventStoreDbIntegration
{
    public interface IEventStoreDbProducerCache<TKey>
    {
        Task<IEventStoreDbProducer> GetProducer(TKey key, Func<TKey, Task<IEventStoreDbProducer>> factory);
    }
}
