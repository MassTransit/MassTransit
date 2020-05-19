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

        static void AddMassTransitComponents(IServiceCollection collection)
        {
            collection.TryAddScoped<ScopedConsumeContextProvider>();
            collection.TryAddScoped(provider => provider.GetRequiredService<ScopedConsumeContextProvider>().GetContext() ?? new MissingConsumeContext());
            collection.TryAddSingleton<IConsumerScopeProvider>(provider => new DependencyInjectionConsumerScopeProvider(provider));
            collection.TryAddSingleton<IConfigurationServiceProvider>(provider => new DependencyInjectionConfigurationServiceProvider(provider));
        }

        public void ConfigureMediator(Action<IServiceProvider, IReceiveEndpointConfigurator> configure)
        {
            if(configure == null)
                throw new ArgumentNullException(nameof(configure));

            ThrowIfAlreadyConfigured();
            _configure = configure;
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
