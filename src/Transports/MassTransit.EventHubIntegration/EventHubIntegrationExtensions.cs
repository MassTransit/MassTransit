namespace MassTransit
{
    using System;
    using DependencyInjection;
    using EventHubIntegration;
    using EventHubIntegration.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;


    public static class EventHubIntegrationExtensions
    {
        public static void UsingEventHub(this IRiderRegistrationConfigurator configurator,
            Action<IRiderRegistrationContext, IEventHubFactoryConfigurator> configure)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var factory = new EventHubRegistrationRiderFactory(configure);
            configurator.SetRiderFactory(factory);

            configurator.TryAddScoped(GetCurrentProducerProvider);
        }

        static IEventHubProducerProvider GetCurrentProducerProvider(IServiceProvider provider)
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
