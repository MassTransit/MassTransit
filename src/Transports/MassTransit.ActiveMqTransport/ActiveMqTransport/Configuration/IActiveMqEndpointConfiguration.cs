namespace MassTransit.ActiveMqTransport.Configuration
{
    using MassTransit.Configuration;


    public interface IActiveMqEndpointConfiguration :
        IEndpointConfiguration
    {
        new IActiveMqTopologyConfiguration Topology { get; }
    }
}
