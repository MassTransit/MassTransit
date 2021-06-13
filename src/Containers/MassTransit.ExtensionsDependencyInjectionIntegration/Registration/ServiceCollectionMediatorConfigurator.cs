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
        Action<IMediatorRegistrationContext, IMediatorConfigurator> _configure;

        public ServiceCollectionMediatorConfigurator(IServiceCollection collection)
            : base(new DependencyInjectionMediatorContainerRegistrar(collection))
        {
            IMediatorRegistrationContext CreateRegistrationContext(IServiceProvider provider)
            {
                var registration = CreateRegistration(provider.GetRequiredService<IConfigurationServiceProvider>(), null);
                return new MediatorRegistrationContext(registration);
            }

            Collection = collection;

            Collection.AddSingleton(MediatorFactory);
            Collection.AddSingleton(CreateRegistrationContext);
            AddMassTransitComponents(collection);
        }

        public IServiceCollection Collection { get; }

        public void ConfigureMediator(Action<IMediatorRegistrationContext, IMediatorConfigurator> configure)
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
            collection.TryAddSingleton<IConfigurationServiceProvider>(provider => new DependencyInjectionConfigurationServiceProvider(provider));
        }

        IMediator MediatorFactory(IServiceProvider serviceProvider)
        {
            var provider = serviceProvider.GetRequiredService<IConfigurationServiceProvider>();

            ConfigureLogContext(provider);

            var context = serviceProvider.GetRequiredService<IMediatorRegistrationContext>();

            return Bus.Factory.CreateMediator(cfg =>
            {
                _configure?.Invoke(context, cfg);
                cfg.ConfigureConsumers(context);
                cfg.ConfigureSagas(context);
            });
        }
    }
}
