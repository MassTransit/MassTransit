using MassTransit.Registration;

namespace MassTransit.Transports.Outbox
{
    public static class DependencyInjectionOutboxExtensions
    {
        public static void UseOutboxTransport(this IBusFactoryConfigurator configurator, IConfigurationServiceProvider serviceProvider)
        {
            configurator.UsePublishFilter(typeof(OnRampScopedRepositoryFilter<>), serviceProvider);
            configurator.UseSendFilter(typeof(OnRampScopedRepositoryFilter<>), serviceProvider);
            configurator.UseOutboxTransport = true;
            configurator.AutoStart = true; // This is for the the IBusControl Health checks to know if we need to stop trying while the bus is down
        }
    }
}
