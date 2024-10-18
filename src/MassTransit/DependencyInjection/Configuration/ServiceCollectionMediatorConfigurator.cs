namespace MassTransit.Configuration
{
    using System;
    using Context;
    using DependencyInjection;
    using Mediator;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;


    public class ServiceCollectionMediatorConfigurator :
        RegistrationConfigurator,
        IMediatorRegistrationConfigurator
    {
        Action<IMediatorRegistrationContext, IMediatorConfigurator> _configure;

        public ServiceCollectionMediatorConfigurator(IServiceCollection collection, Uri baseAddress)
            : base(collection, new DependencyInjectionMediatorContainerRegistrar(collection))
        {
            IMediatorRegistrationContext CreateRegistrationContext(IServiceProvider provider)
            {
                var setter = provider.GetRequiredService<Bind<IMediator, ISetScopedConsumeContext>>();
                var registration = CreateRegistration(provider, setter.Value);
                return new MediatorRegistrationContext(registration);
            }

            collection.AddSingleton(e => MediatorFactory(e, baseAddress));
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
            collection.AddScoped<IScopedMediator, ScopedMediator>();

            collection.TryAddScoped<ScopedConsumeContextProvider>();
            collection.TryAddScoped<IScopedConsumeContextProvider>(provider => provider.GetRequiredService<ScopedConsumeContextProvider>());
            collection.AddSingleton(_ =>
                Bind<IMediator>.Create((ISetScopedConsumeContext)new SetScopedConsumeContext(provider =>
                    provider.GetRequiredService<Bind<IMediator, IScopedConsumeContextProvider>>().Value)));

            static Bind<IMediator, IScopedConsumeContextProvider> CreateScopeProvider(IServiceProvider provider)
            {
                var global = provider.GetRequiredService<IScopedConsumeContextProvider>();
                return Bind<IMediator>.Create((IScopedConsumeContextProvider)new TypedScopedConsumeContextProvider(global));
            }

            collection.TryAddScoped(CreateScopeProvider);
            collection.TryAddScoped(provider => provider.GetRequiredService<IScopedConsumeContextProvider>().GetContext() ?? MissingConsumeContext.Instance);

            collection.TryAddScoped(typeof(IRequestClient<>), typeof(GenericRequestClient<>));
        }

        IMediator MediatorFactory(IServiceProvider provider, Uri baseAddress)
        {
            ConfigureLogContext(provider);

            var context = provider.GetRequiredService<IMediatorRegistrationContext>();

            return Bus.Factory.CreateMediator(baseAddress, cfg =>
            {
                _configure?.Invoke(context, cfg);

                cfg.ConfigureConsumers(context);
                cfg.ConfigureSagas(context);
            });
        }
    }
}
