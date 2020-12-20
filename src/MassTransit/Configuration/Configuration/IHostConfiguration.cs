namespace MassTransit.Configuration
{
    using System;
    using Context;
    using EndpointConfigurators;
    using GreenPipes;
    using GreenPipes.Agents;
    using Topology;


    public interface IHostConfiguration :
        IEndpointConfigurationObserverConnector,
        IReceiveObserverConnector,
        IConsumeObserverConnector,
        IPublishObserverConnector,
        ISendObserverConnector,
        ISpecification
    {
        IAgent Agent { get; }

        IBusConfiguration BusConfiguration { get; }

        Uri HostAddress { get; }

        /// <summary>
        /// If true, only the broker topology will be deployed
        /// </summary>
        bool DeployTopologyOnly { get; set; }

        ILogContext LogContext { get; set; }
        ILogContext ReceiveLogContext { get; }
        ILogContext SendLogContext { get; }

        IHostTopology HostTopology { get; }

        IRetryPolicy ReceiveTransportRetryPolicy { get; }

        /// <summary>
        /// Create a receive endpoint configuration
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        IReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName, Action<IReceiveEndpointConfigurator> configure = null);

        /// <summary>
        /// Called by the base ReceiveEndpointContext constructor so that the observer collections are connected to the bus observer
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        ConnectHandle ConnectReceiveEndpointContext(ReceiveEndpointContext context);

        IHost Build();
    }
}
