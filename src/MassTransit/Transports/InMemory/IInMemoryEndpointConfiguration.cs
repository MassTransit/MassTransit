namespace MassTransit.Transports.InMemory
{
    using EndpointSpecifications;
    using MassTransit.Topology.Configuration;
    using Topology;
    using Topology.Configurators;


    public interface IInMemoryEndpointConfiguration :
        IEndpointConfiguration<IInMemoryEndpointConfiguration, IInMemoryConsumeTopologyConfigurator, ISendTopologyConfigurator, IInMemoryPublishTopologyConfigurator>
    {
    }
}