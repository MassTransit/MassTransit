namespace MassTransit.ExtensionsDependencyInjectionIntegration.Registration
{
    using System;
    using Context;
    using MassTransit.Registration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using ScopeProviders;
    using Scoping;


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
            Collection.AddSingleton<ISendEndpointProvider>(provider => provider.GetRequiredService<IBusControl>());
            Collection.AddSingleton<IPublishEndpoint>(provider => provider.GetRequiredService<IBusControl>());

            Collection.AddSingleton(context => context.GetRequiredService<IBus>().CreateClientFactory());
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

            collection.AddScoped(provider => (ISendEndpointProvider)provider.GetService<ScopedConsumeContextProvider>()?.GetContext() ??
                provider.GetRequiredService<IBus>());

            collection.AddScoped(provider => (IPublishEndpoint)provider.GetService<ScopedConsumeContextProvider>()?.GetContext() ??
                provider.GetRequiredService<IBus>());

            collection.AddSingleton<IConsumerScopeProvider>(provider => new DependencyInjectionConsumerScopeProvider(provider));
            collection.AddSingleton<ISagaRepositoryFactory>(provider => new DependencyInjectionSagaRepositoryFactory(provider));
            collection.AddSingleton<IConfigurationServiceProvider>(provider => new DependencyInjectionConfigurationServiceProvider(provider));
        }
    }
}
