namespace MassTransit.Transports.InMemory
{
    using EndpointSpecifications;
    using MassTransit.Topology.Configuration;
    using Topology;


    public interface IInMemoryEndpointConfiguration :
        IEndpointConfiguration<IInMemoryEndpointConfiguration, IInMemoryConsumeTopologyConfigurator, ISendTopologyConfigurator, IInMemoryPublishTopologyConfigurator>
    {
    }
}