namespace MassTransit.EventHubIntegration
{
    using System;
    using Registration;


    public static class EventHubIntegrationExtensions
    {
        public static void UsingEventHub<TContainerContext>(this IRiderConfigurator<TContainerContext> configurator,
            Action<IRiderRegistrationContext<TContainerContext>, IEventHubFactoryConfigurator> configure)
            where TContainerContext : class
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var factory = new EventHubRegistrationRiderFactory<TContainerContext>(configure);
            configurator.SetRiderFactory(factory);
        }
    }
}
