namespace MassTransit.Configuration
{
    using Transports;


    public interface ISendPipeConfiguration
    {
        ISendPipeSpecification Specification { get; }
        ISendPipeConfigurator Configurator { get; }

        ISendPipe CreatePipe();
    }
}
