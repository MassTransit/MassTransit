namespace MassTransit.ExtensionsDependencyInjectionIntegration.Registration
{
    using System;
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
            Collection.TryAddSingleton(busFactory);

            Collection.AddSingleton<IBus>(provider => provider.GetRequiredService<IBusControl>());
            Collection.AddSingleton<ISendEndpointProvider>(provider => provider.GetRequiredService<IBusControl>());
            Collection.AddSingleton<IPublishEndpoint>(provider => provider.GetRequiredService<IBusControl>());

            Collection.AddSingleton(context => context.GetRequiredService<IBus>().CreateClientFactory());
        }

        static void AddMassTransitComponents(IServiceCollection collection)
        {
            collection.AddScoped<ScopedConsumeContextProvider>();
            collection.AddScoped(provider => provider.GetRequiredService<ScopedConsumeContextProvider>().GetContext());

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
