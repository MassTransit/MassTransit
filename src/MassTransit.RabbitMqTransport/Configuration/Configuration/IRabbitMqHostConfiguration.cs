namespace MassTransit.RabbitMqTransport.Configuration
{
    using System.Threading.Tasks;
    using MassTransit.Configuration;
    using Topology;
    using Topology.Settings;
    using Transport;
    using Transports;


    public interface IRabbitMqHostConfiguration :
        IHostConfiguration
    {
        IRabbitMqBusConfiguration BusConfiguration { get; }

        string Description { get; }

        new IRabbitMqHostControl Host { get; }

        /// <summary>
        /// Create a receive endpoint configuration using the specified host
        /// </summary>
        /// <returns></returns>
        IRabbitMqReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName);

        IRabbitMqReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(RabbitMqReceiveSettings settings,
            IRabbitMqEndpointConfiguration endpointConfiguration);

        new IRabbitMqHostTopology Topology { get; }

        /// <summary>
        /// True if the broker is confirming published messages
        /// </summary>
        bool PublisherConfirmation { get; }

        RabbitMqHostSettings Settings { get; }

        Task<ISendTransport> CreatePublishTransport<T>()
            where T : class;
    }
}
