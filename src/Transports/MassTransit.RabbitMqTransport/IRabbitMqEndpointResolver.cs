namespace MassTransit.RabbitMqTransport
{
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
