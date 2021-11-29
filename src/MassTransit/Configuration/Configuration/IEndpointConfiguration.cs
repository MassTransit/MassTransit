namespace MassTransit.Configuration
{
    public interface IEndpointConfiguration :
        IConsumePipeConfigurator,
        ISendPipelineConfigurator,
        IPublishPipelineConfigurator,
        IReceivePipelineConfigurator,
        ISpecification
    {
        IConsumePipeConfiguration Consume { get; }
        ISendPipeConfiguration Send { get; }
        IPublishPipeConfiguration Publish { get; }
        IReceivePipeConfiguration Receive { get; }

        ITopologyConfiguration Topology { get; }

        ISerializationConfiguration Serialization { get; }

        ITransportConfiguration Transport { get; }
    }
}
