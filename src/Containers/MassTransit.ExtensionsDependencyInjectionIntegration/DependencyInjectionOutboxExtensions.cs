using MassTransit.Registration;

namespace MassTransit.Transports.Outbox
{
    public static class DependencyInjectionOutboxExtensions
    {
        public static void UseOutboxTransport(this IBusFactoryConfigurator configurator, IConfigurationServiceProvider serviceProvider)
        {
            configurator.UsePublishFilter(typeof(OutboxScopedRepositoryFilter<>), serviceProvider);
            configurator.UseSendFilter(typeof(OutboxScopedRepositoryFilter<>), serviceProvider);
            configurator.UseOutboxTransport = true;
            configurator.AutoStart = true; // This is for the the IBusHealth checks to know if we need to stop trying while the bus is down
        }
    }
}
