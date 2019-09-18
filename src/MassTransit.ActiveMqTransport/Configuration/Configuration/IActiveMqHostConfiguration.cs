namespace MassTransit.ActiveMqTransport.Configuration
{
    using System;
    using MassTransit.Configuration;
    using Topology.Settings;


    public interface IActiveMqHostConfiguration :
        IHostConfiguration,
        IReceiveConfigurator<IActiveMqReceiveEndpointConfigurator>
    {
        string Description { get; }

        ActiveMqHostSettings Settings { get; set; }

        /// <summary>
        /// If true, only the broker topology will be deployed
        /// </summary>
        bool DeployTopologyOnly { get; set; }

        IActiveMqHost Proxy { get; }

        /// <summary>
        /// Create a receive endpoint configuration for the default host
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        IActiveMqReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName,
            Action<IActiveMqReceiveEndpointConfigurator> configure = null);

        /// <summary>
        /// Create a receive endpoint configuration for the default host
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="endpointConfiguration"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        IActiveMqReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(QueueReceiveSettings settings,
            IActiveMqEndpointConfiguration endpointConfiguration, Action<IActiveMqReceiveEndpointConfigurator> configure = null);
    }
}
