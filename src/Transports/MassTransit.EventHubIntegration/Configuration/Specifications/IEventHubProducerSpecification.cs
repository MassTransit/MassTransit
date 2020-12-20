namespace MassTransit.EventHubIntegration.Specifications
{
    using GreenPipes;
    using MassTransit.Registration;


    public interface IEventHubProducerSpecification :
        ISpecification
    {
        IEventHubProducerProvider CreateProducerProvider(IBusInstance busInstance);
    }
}
