namespace MassTransit.RabbitMqTransport.Configuration
{
    using System;
    using MassTransit.Configuration;
    using Topology.Settings;


    public interface IRabbitMqHostConfiguration :
        IHostConfiguration,
        IReceiveConfigurator<IRabbitMqReceiveEndpointConfigurator>
    {
        string Description { get; }

        RabbitMqHostSettings Settings { get; set; }

        /// <summary>
        /// True if the broker is confirming published messages
        /// </summary>
        bool PublisherConfirmation { get; }

        BatchSettings BatchSettings { get; }

        /// <summary>
        /// If true, only the broker topology will be deployed
        /// </summary>
        bool DeployTopologyOnly { get; set; }

        IRabbitMqHost Proxy { get; }

        /// <summary>
        /// Apply the endpoint definition to the receive endpoint configurator
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="definition"></param>
        void ApplyEndpointDefinition(IRabbitMqReceiveEndpointConfigurator configurator, IEndpointDefinition definition);

        IRabbitMqReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName,
            Action<IRabbitMqReceiveEndpointConfigurator> configure = null);

        IRabbitMqReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(RabbitMqReceiveSettings settings,
            IRabbitMqEndpointConfiguration endpointConfiguration, Action<IRabbitMqReceiveEndpointConfigurator> configure = null);
    }
}
