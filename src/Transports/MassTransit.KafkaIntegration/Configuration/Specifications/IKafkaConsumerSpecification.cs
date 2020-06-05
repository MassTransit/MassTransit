namespace MassTransit.KafkaIntegration.Specifications
{
    using GreenPipes;
    using MassTransit.Registration;
    using Transport;
    using Transports;


    public interface IKafkaConsumerSpecification :
        IReceiveEndpointObserverConnector,
        ISpecification
    {
        string Name { get; }

        /// <summary>
        /// Create the receive endpoint, using the busInstance hostConfiguration
        /// </summary>
        /// <param name="busInstance"></param>
        /// <returns></returns>
        IKafkaReceiveEndpoint CreateReceiveEndpoint(IBusInstance busInstance);
    }
}
