namespace MassTransit.Configuration
{
    using Transports;


    public interface IPublishPipeConfiguration
    {
        IPublishPipeSpecification Specification { get; }
        IPublishPipeConfigurator Configurator { get; }

        IPublishPipe CreatePipe();
    }
}
