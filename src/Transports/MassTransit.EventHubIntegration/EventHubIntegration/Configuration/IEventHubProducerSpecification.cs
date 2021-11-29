namespace MassTransit.EventHubIntegration.Configuration
{
    using Transports;


    public interface IEventHubProducerSpecification :
        ISpecification
    {
        IEventHubProducerProvider CreateProducerProvider(IBusInstance busInstance);
    }
}
