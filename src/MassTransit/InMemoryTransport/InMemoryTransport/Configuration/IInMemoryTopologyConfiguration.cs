namespace MassTransit.InMemoryTransport.Configuration
{
    using MassTransit.Configuration;


    public interface IInMemoryTopologyConfiguration :
        ITopologyConfiguration
    {
        new IInMemoryPublishTopologyConfigurator Publish { get; }

        new IInMemoryConsumeTopologyConfigurator Consume { get; }
    }
}
