namespace MassTransit.SimpleInjectorIntegration.Registration
{
    using System;
    using Context;
    using MassTransit.Registration;
    using Mediator;
    using ScopeProviders;
    using Scoping;
    using SimpleInjector;


    public class SimpleInjectorMediatorConfigurator :
        RegistrationConfigurator,
        ISimpleInjectorMediatorConfigurator
    {
        Action<IMediatorRegistrationContext, IMediatorConfigurator> _configure;

        public SimpleInjectorMediatorConfigurator(Container container)
            : base(new SimpleInjectorContainerMediatorRegistrar(container))
        {
            IMediatorRegistrationContext CreateRegistrationContext()
            {
                var registration = CreateRegistration(Container.GetInstance<IConfigurationServiceProvider>());
                return new MediatorRegistrationContext(registration);
            }

            Container = container;

            Container.RegisterSingleton(MediatorFactory);
            Container.RegisterSingleton(CreateRegistrationContext);
            AddMassTransitComponents(Container);
        }

        public Container Container { get; }

        public void ConfigureMediator(Action<IMediatorConfigurator> configure)
        {
            ConfigureMediator((_, cfg) => configure(cfg));
        }

        public void ConfigureMediator(Action<IMediatorRegistrationContext, IMediatorConfigurator> configure)
        {
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            ThrowIfAlreadyConfigured(nameof(ConfigureMediator));
            _configure = configure;
        }

        IMediator MediatorFactory()
        {
            var provider = Container.GetInstance<IConfigurationServiceProvider>();

            ConfigureLogContext(provider);

            var context = Container.GetInstance<IMediatorRegistrationContext>();

            return Bus.Factory.CreateMediator(cfg =>
            {
                _configure?.Invoke(context, cfg);
                cfg.ConfigureConsumers(context);
                cfg.ConfigureSagas(context);
            });
        }

        static void AddMassTransitComponents(Container container)
        {
            container.Register<ScopedConsumeContextProvider>(Lifestyle.Scoped);

            container.Register(() => container.GetInstance<ScopedConsumeContextProvider>().GetContext() ?? new MissingConsumeContext(),
                Lifestyle.Scoped);

            container.RegisterSingleton<IConsumerScopeProvider>(() => new SimpleInjectorConsumerScopeProvider(container));
            container.RegisterSingleton<IConfigurationServiceProvider>(() => new SimpleInjectorConfigurationServiceProvider(container));
        }
    }
}
