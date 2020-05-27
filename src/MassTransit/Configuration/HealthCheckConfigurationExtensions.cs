namespace MassTransit
{
    using Attachments;
    using Registration;


    public static class HealthCheckConfigurationExtensions
    {
        public static void UseHealthCheck<TContainerContext>(this IBusFactoryConfigurator configurator,
            IRegistrationContext<TContainerContext> context)
            where TContainerContext : class
        {
            context.UseHealthCheck(configurator);
        }

        public static void UseHealthCheck<TContainerContext>(this IBusAttachmentFactoryConfigurator configurator,
            IBusAttachmentRegistrationContext<TContainerContext> context)
            where TContainerContext : class
        {
            context.UseHealthCheck(configurator);
        }
    }
}
