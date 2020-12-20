namespace MassTransit.KafkaIntegration.Specifications
{
    using GreenPipes;
    using MassTransit.Registration;
    using Transports;


    public interface IKafkaConsumerSpecification :
        IReceiveEndpointObserverConnector,
        ISpecification
    {
        string QueueName { get; }

        /// <summary>
        /// Create the receive endpoint, using the busInstance hostConfiguration
        /// </summary>
        /// <param name="busInstance"></param>
        /// <returns></returns>
        IReceiveEndpointControl CreateReceiveEndpoint(IBusInstance busInstance);
    }
}
