namespace MassTransit.Configuration
{
    using System;
    using DependencyInjection;
    using Mediator;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;


    public class ServiceCollectionMediatorConfigurator :
        RegistrationConfigurator,
        IMediatorRegistrationConfigurator
    {
        Action<IMediatorRegistrationContext, IMediatorConfigurator> _configure;

        public ServiceCollectionMediatorConfigurator(IServiceCollection collection)
            : base(collection, new DependencyInjectionMediatorContainerRegistrar(collection))
        {
            IMediatorRegistrationContext CreateRegistrationContext(IServiceProvider provider)
            {
                var registration = CreateRegistration(provider);
                return new MediatorRegistrationContext(registration);
            }

            collection.AddSingleton(MediatorFactory);
            collection.AddSingleton(CreateRegistrationContext);

            AddMassTransitComponents(collection);
        }

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
            collection.TryAddSingleton<IConsumeScopeProvider>(provider => new ConsumeScopeProvider(provider));
        }

        IMediator MediatorFactory(IServiceProvider provider)
        {
            ConfigureLogContext(provider);

            var context = provider.GetRequiredService<IMediatorRegistrationContext>();

            return Bus.Factory.CreateMediator(cfg =>
            {
                _configure?.Invoke(context, cfg);
                cfg.ConfigureConsumers(context);
                cfg.ConfigureSagas(context);
            });
        }
    }
}
