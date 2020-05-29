namespace MassTransit.Configuration
{
    using Pipeline;
    using SendPipeSpecifications;


    public interface ISendPipeConfiguration
    {
        ISendPipeSpecification Specification { get; }
        ISendPipeConfigurator Configurator { get; }

        ISendPipe CreatePipe();
    }
}
