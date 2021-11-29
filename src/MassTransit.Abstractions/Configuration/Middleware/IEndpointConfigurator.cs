namespace MassTransit
{
    public interface IEndpointConfigurator :
        IConsumePipeConfigurator,
        ISendPipelineConfigurator,
        IPublishPipelineConfigurator,
        IReceivePipelineConfigurator
    {
    }
}
