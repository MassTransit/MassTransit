namespace MassTransit.SqlTransport.Configuration
{
    using MassTransit.Configuration;


    public interface ISqlTopologyConfiguration :
        ITopologyConfiguration
    {
        new ISqlPublishTopologyConfigurator Publish { get; }

        new ISqlSendTopologyConfigurator Send { get; }

        new ISqlConsumeTopologyConfigurator Consume { get; }
    }
}
