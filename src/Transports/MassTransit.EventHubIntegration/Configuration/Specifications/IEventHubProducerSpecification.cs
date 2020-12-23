namespace MassTransit.EventHubIntegration.Specifications
{
    using GreenPipes;
    using MassTransit.Registration;


    public interface IEventHubProducerSpecification :
        ISpecification
    {
        IEvenHubProducerProviderFactory CreateProducerProviderFactory(IBusInstance busInstance);
    }
}
