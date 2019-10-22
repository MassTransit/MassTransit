namespace MassTransit.AmazonSqsTransport.Configuration.Configuration
{
    using System;
    using MassTransit.Configuration;
    using Topology.Settings;


    public interface IAmazonSqsHostConfiguration :
        IHostConfiguration,
        IReceiveConfigurator<IAmazonSqsReceiveEndpointConfigurator>
    {
        AmazonSqsHostSettings Settings { get; set; }

        /// <summary>
        /// If true, only the broker topology will be deployed
        /// </summary>
        bool DeployTopologyOnly { get; set; }

        IAmazonSqsHost Proxy { get; }

        /// <summary>
        /// Create a receive endpoint configuration using the specified host
        /// </summary>
        /// <returns></returns>
        IAmazonSqsReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName,
            Action<IAmazonSqsReceiveEndpointConfigurator> configure = null);

        /// <summary>
        /// Create a receive endpoint configuration for the default host
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="endpointConfiguration"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        IAmazonSqsReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(QueueReceiveSettings settings,
            IAmazonSqsEndpointConfiguration endpointConfiguration, Action<IAmazonSqsReceiveEndpointConfigurator> configure = null);
    }
}
