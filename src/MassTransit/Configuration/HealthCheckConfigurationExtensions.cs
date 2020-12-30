namespace MassTransit
{
    public static class HealthCheckConfigurationExtensions
    {
        public static void UseHealthCheck(this IBusFactoryConfigurator configurator, IBusRegistrationContext context)
        {
            context.UseHealthCheck(configurator);
        }
    }
}
