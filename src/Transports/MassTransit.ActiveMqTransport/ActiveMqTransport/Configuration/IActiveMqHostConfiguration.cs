namespace MassTransit.ActiveMqTransport.Configuration
{
    using System;
    using MassTransit.Configuration;


    public interface IActiveMqHostConfiguration :
        IHostConfiguration,
        IReceiveConfigurator<IActiveMqReceiveEndpointConfigurator>
    {
        ActiveMqHostSettings Settings { get; set; }

        bool IsArtemis { get; set; }

        IConnectionContextSupervisor ConnectionContextSupervisor { get; }

        new IActiveMqBusTopology Topology { get; }

        /// <summary>
        /// Apply the endpoint definition to the receive endpoint configurator
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="definition"></param>
        void ApplyEndpointDefinition(IActiveMqReceiveEndpointConfigurator configurator, IEndpointDefinition definition);

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
        IActiveMqReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(ActiveMqQueueReceiveSettings settings,
            IActiveMqEndpointConfiguration endpointConfiguration, Action<IActiveMqReceiveEndpointConfigurator> configure = null);
    }
}
