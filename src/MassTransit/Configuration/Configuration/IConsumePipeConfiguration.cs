namespace MassTransit.Configuration
{
    using ConsumePipeSpecifications;


    public interface IConsumePipeConfiguration
    {
        IConsumePipeSpecification Specification { get; }
        IConsumePipeConfigurator Configurator { get; }
    }
}
