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
        Action<IRegistration, IReceiveEndpointConfigurator> _configure;

        public SimpleInjectorMediatorConfigurator(Container container)
            : base(new SimpleInjectorContainerMediatorRegistrar(container))
        {
            Container = container;

            Container.RegisterSingleton(MediatorFactory);
            AddMassTransitComponents(Container);
        }

        public Container Container { get; }

        public void ConfigureMediator(Action<IReceiveEndpointConfigurator> configure)
        {
            ConfigureMediator((_, cfg) => configure(cfg));
        }

        public void ConfigureMediator(Action<IRegistration, IReceiveEndpointConfigurator> configure)
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

            return Bus.Factory.CreateMediator(cfg =>
            {
                base.ConfigureMediator(cfg, provider, _configure);
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
