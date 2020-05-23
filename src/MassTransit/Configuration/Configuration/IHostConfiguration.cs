namespace MassTransit.Configuration
{
    using System;
    using Context;
    using EndpointConfigurators;
    using GreenPipes;


    public interface IHostConfiguration :
        IEndpointConfigurationObserverConnector,
        ISpecification
    {
        IBusConfiguration BusConfiguration { get; }

        Uri HostAddress { get; }

        /// <summary>
        /// If true, only the broker topology will be deployed
        /// </summary>
        bool DeployTopologyOnly { get; set; }

        ILogContext LogContext { get; set; }
        ILogContext SendLogContext { get; }
        ILogContext ReceiveLogContext { get; }

        /// <summary>
        /// Create a receive endpoint configuration
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        IReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName, Action<IReceiveEndpointConfigurator> configure = null);

        IHost Build();
    }
}
