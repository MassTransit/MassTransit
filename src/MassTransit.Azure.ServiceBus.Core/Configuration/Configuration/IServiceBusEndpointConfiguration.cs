namespace MassTransit.Azure.ServiceBus.Core.Configuration
{
    using MassTransit.Configuration;


    public interface IServiceBusEndpointConfiguration :
        IEndpointConfiguration
    {
        new IServiceBusTopologyConfiguration Topology { get; }
    }
}
