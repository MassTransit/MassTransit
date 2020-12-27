namespace MassTransit.KafkaIntegration.Specifications
{
    using GreenPipes;
    using MassTransit.Registration;
    using Transports;


    public interface IKafkaConsumerSpecification :
        IReceiveEndpointObserverConnector,
        ISpecification
    {
        string EndpointName { get; }

        /// <summary>
        /// Create the receive endpoint, using the busInstance hostConfiguration
        /// </summary>
        /// <param name="busInstance"></param>
        /// <returns></returns>
        ReceiveEndpoint CreateReceiveEndpoint(IBusInstance busInstance);
    }
}
