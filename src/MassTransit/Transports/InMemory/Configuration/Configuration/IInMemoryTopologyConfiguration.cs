namespace MassTransit.Transports.InMemory.Configuration
{
    using MassTransit.Configuration;
    using Topology.Configurators;


    public interface IInMemoryTopologyConfiguration :
        ITopologyConfiguration
    {
        new IInMemoryPublishTopologyConfigurator Publish { get; }

        new IInMemoryConsumeTopologyConfigurator Consume { get; }
    }
}
