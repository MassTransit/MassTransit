namespace MassTransit
{
    public static class HealthCheckConfigurationExtensions
    {
        public static void UseHealthCheck<TContainerContext>(this IBusFactoryConfigurator configurator,
            IRegistrationContext<TContainerContext> context)
            where TContainerContext : class
        {
            context.UseHealthCheck(configurator);
        }
    }
}
