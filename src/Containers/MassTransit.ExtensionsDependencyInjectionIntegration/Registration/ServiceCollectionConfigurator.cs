namespace MassTransit.ExtensionsDependencyInjectionIntegration.Registration
{
    using System;
    using Context;
    using MassTransit.Registration;
    using Mediator;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using ScopeProviders;
    using Scoping;
    using Transports;


    public class ServiceCollectionConfigurator :
        RegistrationConfigurator,
        IServiceCollectionConfigurator
    {
        public ServiceCollectionConfigurator(IServiceCollection collection)
            : base(new DependencyInjectionContainerRegistrar(collection))
        {
            Collection = collection;

            AddMassTransitComponents(collection);

            collection.AddSingleton<IRegistrationConfigurator>(this);
            collection.AddSingleton(provider => CreateRegistration(provider.GetRequiredService<IConfigurationServiceProvider>()));
        }

        public IServiceCollection Collection { get; }

        public void AddBus(Func<IServiceProvider, IBusControl> busFactory)
        {
            IBusControl BusFactory(IServiceProvider serviceProvider)
            {
                var provider = serviceProvider.GetRequiredService<IConfigurationServiceProvider>();

                ConfigureLogContext(provider);

                return busFactory(serviceProvider);
            }

            Collection.TryAddSingleton(BusFactory);

            Collection.AddSingleton<IBus>(provider => provider.GetRequiredService<IBusControl>());

            Collection.AddSingleton(provider => ClientFactoryProvider(provider.GetRequiredService<IConfigurationServiceProvider>()));
        }

        public void AddMediator(Action<IServiceProvider, IReceiveEndpointConfigurator> configure = null)
        {
            IMediator MediatorFactory(IServiceProvider serviceProvider)
            {
                var provider = serviceProvider.GetRequiredService<IConfigurationServiceProvider>();

                ConfigureLogContext(provider);

                return Bus.Factory.CreateMediator(cfg =>
                {
                    configure?.Invoke(serviceProvider, cfg);

                    ConfigureMediator(cfg, provider);
                });
            }

            Collection.TryAddSingleton(MediatorFactory);
            Collection.AddSingleton<IClientFactory>(provider => provider.GetRequiredService<IMediator>());
        }

        static void AddMassTransitComponents(IServiceCollection collection)
        {
            collection.AddScoped<ScopedConsumeContextProvider>();
            collection.AddScoped(provider => provider.GetRequiredService<ScopedConsumeContextProvider>().GetContext() ?? new MissingConsumeContext());

            collection.AddScoped(GetCurrentSendEndpointProvider);
            collection.AddScoped(GetCurrentPublishEndpoint);

            collection.AddSingleton<IConsumerScopeProvider>(provider => new DependencyInjectionConsumerScopeProvider(provider));
            collection.AddSingleton<ISagaRepositoryFactory>(provider => new DependencyInjectionSagaRepositoryFactory(provider));
            collection.AddSingleton<IConfigurationServiceProvider>(provider => new DependencyInjectionConfigurationServiceProvider(provider));
        }

        static ISendEndpointProvider GetCurrentSendEndpointProvider(IServiceProvider provider)
        {
            return (ISendEndpointProvider)provider.GetService<ScopedConsumeContextProvider>()?.GetContext()
                ?? new ScopedSendEndpointProvider<IServiceProvider>(provider.GetRequiredService<IBus>(), provider);
        }

        static IPublishEndpoint GetCurrentPublishEndpoint(IServiceProvider provider)
        {
            return (IPublishEndpoint)provider.GetService<ScopedConsumeContextProvider>()?.GetContext() ?? new PublishEndpoint(
                new ScopedPublishEndpointProvider<IServiceProvider>(provider.GetRequiredService<IBus>(), provider));
        }
    }
}
