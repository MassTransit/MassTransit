namespace MassTransit.KafkaIntegration.Subscriptions
{
    using GreenPipes;
    using Registration;
    using Transports;


    public interface IKafkaTopic :
        IReceiveEndpointObserverConnector,
        ISpecification
    {
        string Name { get; }
        IKafkaReceiveEndpoint CreateEndpoint(IBusInstance busInstance);
    }
}
