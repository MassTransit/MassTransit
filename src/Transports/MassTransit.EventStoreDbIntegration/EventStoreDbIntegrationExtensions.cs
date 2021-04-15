using System;
using EventStore.Client;
using MassTransit.EventStoreDbIntegration;
using MassTransit.EventStoreDbIntegration.Registration;
using MassTransit.Registration;
using MassTransit.Scoping;

namespace MassTransit
{
    public static class EventStoreDbIntegrationExtensions
    {
        public static void UsingEventStoreDB(this IRiderRegistrationConfigurator configurator,
            Action<IRiderRegistrationContext, IEventStoreDbFactoryConfigurator> configure)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var factory = new EventStoreDbRegistrationRiderFactory(configure);
            configurator.SetRiderFactory(factory);

            configurator.Registrar.Register(GetCurrentProducerProvider);
        }

        static IEventStoreDbProducerProvider GetCurrentProducerProvider(IConfigurationServiceProvider provider)
        {
            var producerProvider = provider.GetRequiredService<IEventStoreDbRider>();

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
