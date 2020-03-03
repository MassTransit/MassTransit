namespace MassTransit.Conductor.Configuration.Configurators
{
    public interface IServiceEndpointConfigurator
    {
        void ConfigureMessage<T>(IConsumePipeConfigurator configurator)
            where T : class;
    }
}
