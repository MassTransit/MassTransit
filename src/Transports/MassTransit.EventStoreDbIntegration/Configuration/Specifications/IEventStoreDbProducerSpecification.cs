using GreenPipes;
using MassTransit.Registration;

namespace MassTransit.EventStoreDbIntegration.Specifications
{
    public interface IEventStoreDbProducerSpecification :
        ISpecification
    {
        IEventStoreDbProducerProvider CreateProducerProvider(IBusInstance busInstance);
    }
}
