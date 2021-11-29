namespace MassTransit.GrpcTransport.Configuration
{
    using MassTransit.Configuration;


    public interface IGrpcTopologyConfiguration :
        ITopologyConfiguration
    {
        new IGrpcPublishTopologyConfigurator Publish { get; }

        new IGrpcConsumeTopologyConfigurator Consume { get; }
    }
}
