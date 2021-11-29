namespace MassTransit.RabbitMqTransport
{
    using Configuration;
    using RabbitMQ.Client;


    public interface IRabbitMqEndpointResolver :
        IEndpointResolver
    {
        /// <summary>
        /// Returns the last host selected by the selector
        /// </summary>
        ClusterNode LastHost { get; }
    }
}
