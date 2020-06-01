namespace MassTransit.KafkaIntegration.Specifications
{
    using GreenPipes;
    using MassTransit.Registration;
    using Transport;
    using Transports;


    public interface IKafkaTopicSpecification :
        IReceiveEndpointObserverConnector,
        ISpecification
    {
        string Name { get; }
        IKafkaReceiveEndpoint CreateEndpoint(IBusInstance busInstance);
    }
}
