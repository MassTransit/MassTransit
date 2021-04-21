namespace MassTransit.GrpcTransport.Configuration
{
    using MassTransit.Configuration;
    using Transports.InMemory.Topology.Configurators;


    public interface IGrpcTopologyConfiguration :
        ITopologyConfiguration
    {
        new Topology.Configurators.IGrpcPublishTopologyConfigurator Publish { get; }

        new Topology.Configurators.IGrpcConsumeTopologyConfigurator Consume { get; }
    }
}
