namespace MassTransit
{
    using System;
    using EventHubIntegration;
    using EventHubIntegration.Registration;
    using Registration;
    using Scoping;


    public static class EventHubIntegrationExtensions
    {
        public static void UsingEventHub(this IRiderRegistrationConfigurator configurator,
            Action<IRiderRegistrationContext, IEventHubFactoryConfigurator> configure)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var factory = new EventHubRegistrationRiderFactory(configure);
            configurator.SetRiderFactory(factory);

            configurator.Registrar.Register(GetCurrentProducerProvider);
        }

        static IEventHubProducerProvider GetCurrentProducerProvider(IConfigurationServiceProvider provider)
        {
            var producerProvider = provider.GetRequiredService<IEventHubRider>();

            var contextProvider = provider.GetService<ScopedConsumeContextProvider>();
            if (contextProvider != null)
            {
                return contextProvider.HasContext
                    ? producerProvider.GetProducerProvider(contextProvider.GetContext())
                    : producerProvider.GetProducerProvider();
            }

            return producerProvider.GetProducerProvider(provider.GetService<ConsumeContext>());
        }
    }
}
