namespace MassTransit.Conductor.Configurators
{
    public interface IServiceEndpointConfigurator
    {
        void ConfigureMessage<T>(IConsumePipeConfigurator configurator)
            where T : class;
    }
}
