#nullable enable
namespace MassTransit.Configuration
{
    using System;
    using Logging;
    using Transports;


    public interface IHostConfiguration :
        IEndpointConfigurationObserverConnector,
        IReceiveObserverConnector,
        IConsumeObserverConnector,
        IPublishObserverConnector,
        ISendObserverConnector,
        ISpecification
    {
        IBusConfiguration BusConfiguration { get; }

        Uri HostAddress { get; }

        /// <summary>
        /// If true, only the broker topology will be deployed
        /// </summary>
        bool DeployTopologyOnly { get; set; }

        ISendObserver SendObservers { get; }

        ILogContext? LogContext { get; set; }
        ILogContext? ReceiveLogContext { get; }
        ILogContext? SendLogContext { get; }

        IBusTopology Topology { get; }

        IRetryPolicy ReceiveTransportRetryPolicy { get; }

        /// <summary>
        /// Create a receive endpoint configuration
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        IReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName, Action<IReceiveEndpointConfigurator>? configure = null);

        /// <summary>
        /// Called by the base ReceiveEndpointContext constructor so that the observer collections are connected to the bus observer
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        ConnectHandle ConnectReceiveEndpointContext(ReceiveEndpointContext context);

        IHost Build();
    }
}
