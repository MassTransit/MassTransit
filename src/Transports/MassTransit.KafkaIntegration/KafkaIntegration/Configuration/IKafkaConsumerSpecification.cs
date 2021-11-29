namespace MassTransit.KafkaIntegration.Configuration
{
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
