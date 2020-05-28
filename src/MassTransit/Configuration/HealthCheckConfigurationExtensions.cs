namespace MassTransit
{
    using Registration;
    using Riders;


    public static class HealthCheckConfigurationExtensions
    {
        public static void UseHealthCheck<TContainerContext>(this IBusFactoryConfigurator configurator,
            IRegistrationContext<TContainerContext> context)
            where TContainerContext : class
        {
            context.UseHealthCheck(configurator);
        }

        public static void UseHealthCheck<TContainerContext>(this IRiderFactoryConfigurator configurator,
            IRiderRegistrationContext<TContainerContext> context)
            where TContainerContext : class
        {
            context.UseHealthCheck(configurator);
        }
    }
}
