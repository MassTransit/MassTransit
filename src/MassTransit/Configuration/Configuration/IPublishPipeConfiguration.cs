namespace MassTransit.Configuration
{
    using Pipeline;
    using PublishPipeSpecifications;


    public interface IPublishPipeConfiguration
    {
        IPublishPipeSpecification Specification { get; }
        IPublishPipeConfigurator Configurator { get; }

        IPublishPipe CreatePipe();
    }
}
