namespace MassTransit.ActiveMqTransport.Configuration
{
    using MassTransit.Configuration;


    public interface IActiveMqTopologyConfiguration :
        ITopologyConfiguration
    {
        new IActiveMqPublishTopologyConfigurator Publish { get; }

        new IActiveMqSendTopologyConfigurator Send { get; }

        new IActiveMqConsumeTopologyConfigurator Consume { get; }
    }
}
