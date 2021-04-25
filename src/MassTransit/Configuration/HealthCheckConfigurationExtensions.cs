namespace MassTransit
{
    using System;


    public static class HealthCheckConfigurationExtensions
    {
        [Obsolete("This method is no longer required, health checks are now built into the bus")]
        public static void UseHealthCheck(this IBusFactoryConfigurator configurator, IBusRegistrationContext context)
        {
        }
    }
}
