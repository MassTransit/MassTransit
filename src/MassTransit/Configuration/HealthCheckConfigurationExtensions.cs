namespace MassTransit
{
    using Registration;
    using Riders;


    public static class HealthCheckConfigurationExtensions
    {
        public static void UseHealthCheck(this IBusFactoryConfigurator configurator, IBusRegistrationContext context)
        {
            context.UseHealthCheck(configurator);
        }

        public static void UseHealthCheck(this IRiderFactoryConfigurator configurator, IRiderRegistrationContext context)
        {
            context.UseHealthCheck(configurator);
        }
    }
}
