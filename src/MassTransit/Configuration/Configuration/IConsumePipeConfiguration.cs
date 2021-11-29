namespace MassTransit.Configuration
{
    public interface IConsumePipeConfiguration
    {
        IConsumePipeSpecification Specification { get; }
        IConsumePipeConfigurator Configurator { get; }
    }
}
