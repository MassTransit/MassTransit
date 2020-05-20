namespace MassTransit.Configuration
{
    using System;
    using EndpointConfigurators;
    using GreenPipes;
    using Transports;


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

        IBusHostControl Build();
    }
}
