namespace MassTransit.EventHubIntegration.Specifications
{
    using Contexts;
    using GreenPipes;
    using MassTransit.Registration;


    public interface IEventHubProducerSpecification :
        ISpecification
    {
        IEventHubProducerSharedContext CreateContext(IBusInstance busInstance);
    }
}
