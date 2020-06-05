namespace MassTransit.ExtensionsDependencyInjectionIntegration.Registration
{
    using System;
    using MassTransit.Registration;
    using Mediator;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using ScopeProviders;
    using Scoping;


    public class ServiceCollectionMediatorConfigurator :
        RegistrationConfigurator,
        IServiceCollectionMediatorConfigurator
    {
        Action<IServiceProvider, IReceiveEndpointConfigurator> _configure;

        public ServiceCollectionMediatorConfigurator(IServiceCollection collection)
            : base(new DependencyInjectionMediatorContainerRegistrar(collection))
        {
            Collection = collection;

            Collection.AddSingleton(MediatorFactory);
            AddMassTransitComponents(collection);
        }

        public IServiceCollection Collection { get; }

        public void ConfigureMediator(Action<IServiceProvider, IReceiveEndpointConfigurator> configure)
        {
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            ThrowIfAlreadyConfigured(nameof(ConfigureMediator));
            _configure = configure;
        }

        static void AddMassTransitComponents(IServiceCollection collection)
        {
            collection.TryAddScoped<ScopedConsumeContextProvider>();
            collection.TryAddScoped(provider => provider.GetRequiredService<ScopedConsumeContextProvider>().GetContext());
            collection.TryAddSingleton<IConsumerScopeProvider>(provider => new DependencyInjectionConsumerScopeProvider(provider));
            collection.TryAddTransient<IConfigurationServiceProvider>(provider => new DependencyInjectionConfigurationServiceProvider(provider));
        }

        IMediator MediatorFactory(IServiceProvider serviceProvider)
        {
            var provider = serviceProvider.GetRequiredService<IConfigurationServiceProvider>();

            ConfigureLogContext(provider);

            return Bus.Factory.CreateMediator(cfg =>
            {
                _configure?.Invoke(serviceProvider, cfg);

                ConfigureMediator(cfg, provider);
            });
        }
    }
}
