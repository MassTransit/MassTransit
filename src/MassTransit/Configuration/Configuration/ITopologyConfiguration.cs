namespace MassTransit.Configuration
{
    public interface ITopologyConfiguration :
        ISpecification
    {
        IMessageTopologyConfigurator Message { get; }
        ISendTopologyConfigurator Send { get; }
        IPublishTopologyConfigurator Publish { get; }
        IConsumeTopologyConfigurator Consume { get; }
    }
}
