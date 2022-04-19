namespace MassTransit
{
    using System;
    using DependencyInjection;
    using EventHubIntegration.Configuration;
    using Microsoft.Extensions.DependencyInjection;


    public static class EventHubIntegrationExtensions
    {
        public static void UsingEventHub(this IRiderRegistrationConfigurator configurator,
            Action<IRiderRegistrationContext, IEventHubFactoryConfigurator> configure)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var factory = new EventHubRegistrationRiderFactory(configure);
            configurator.SetRiderFactory(factory);

            configurator.TryAddScoped<IEventHubRider, IEventHubProducerProvider>(GetCurrentProducerProvider);
        }

        public static void UsingEventHub<TBus>(this IRiderRegistrationConfigurator<TBus> configurator,
            Action<IRiderRegistrationContext, IEventHubFactoryConfigurator> configure)
            where TBus : class, IBus
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var factory = new EventHubRegistrationRiderFactory(configure);
            configurator.SetRiderFactory(factory);

            configurator.TryAddScoped<IEventHubRider, Bind<TBus, IEventHubProducerProvider>>((rider, provider) =>
                Bind<TBus>.Create(GetCurrentProducerProvider(rider, provider)));
        }

        static IEventHubProducerProvider GetCurrentProducerProvider(IEventHubRider rider, IServiceProvider provider)
        {
            var contextProvider = provider.GetService<ScopedConsumeContextProvider>();
            if (contextProvider != null)
            {
                return contextProvider.HasContext
                    ? rider.GetProducerProvider(contextProvider.GetContext())
                    : rider.GetProducerProvider();
            }

            return rider.GetProducerProvider(provider.GetService<ConsumeContext>());
        }
    }
}
