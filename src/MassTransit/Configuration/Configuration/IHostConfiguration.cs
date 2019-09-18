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
        Uri HostAddress { get; }

        IBusHostControl Build();
    }
}
