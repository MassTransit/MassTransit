namespace MassTransit.Configuration
{
    using GreenPipes;
    using Topology;


    public interface ITopologyConfiguration :
        ISpecification
    {
        IMessageTopologyConfigurator Message { get; }
        ISendTopologyConfigurator Send { get; }
        IPublishTopologyConfigurator Publish { get; }
        IConsumeTopologyConfigurator Consume { get; }
    }
}
