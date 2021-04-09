using System;
using System.Threading.Tasks;

namespace MassTransit.EventStoreDbIntegration
{
    public interface IEventStoreDbProducerProvider
    {
        Task<IEventStoreDbProducer> GetProducer(Uri address);
    }
}
