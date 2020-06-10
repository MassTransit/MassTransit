namespace MassTransit.WindsorIntegration.Registration
{
    using System;
    using Castle.MicroKernel;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using MassTransit.Registration;
    using Mediator;
    using ScopeProviders;
    using Scoping;


    public class WindsorContainerMediatorConfigurator :
        RegistrationConfigurator,
        IWindsorContainerMediatorConfigurator
    {
        Action<IMediatorRegistrationContext, IReceiveEndpointConfigurator> _configure;

        public WindsorContainerMediatorConfigurator(IWindsorContainer container)
            : base(new WindsorContainerMediatorRegistrar(container))
        {
            IMediatorRegistrationContext CreateRegistrationContext(IKernel kernel)
            {
                var registration = CreateRegistration(kernel.Resolve<IConfigurationServiceProvider>());
                return new MediatorRegistrationContext(registration);
            }

            Container = container;

            container.RegisterScopedContextProviderIfNotPresent();

            if (!container.Kernel.HasComponent(typeof(IConsumerScopeProvider)))
            {
                container.Register(Component.For<IConsumerScopeProvider>()
                    .ImplementedBy<WindsorConsumerScopeProvider>()
                    .LifestyleTransient());
            }

            if (!container.Kernel.HasComponent(typeof(IConfigurationServiceProvider)))
            {
                container.Register(Component.For<IConfigurationServiceProvider>()
                    .ImplementedBy<WindsorConfigurationServiceProvider>()
                    .LifestyleSingleton());
            }

            container.Register(
                Component.For<IMediator>()
                    .UsingFactoryMethod(MediatorFactory)
                    .LifestyleSingleton()
            );

            container.Register(
                Component.For<IMediatorRegistrationContext>()
                    .UsingFactoryMethod(CreateRegistrationContext)
                    .LifestyleSingleton()
            );
        }

        public IWindsorContainer Container { get; }

        public void ConfigureMediator(Action<IMediatorRegistrationContext, IReceiveEndpointConfigurator> configure)
        {
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            ThrowIfAlreadyConfigured(nameof(ConfigureMediator));
            _configure = configure;
        }

        IMediator MediatorFactory(IKernel kernel)
        {
            var provider = kernel.Resolve<IConfigurationServiceProvider>();

            ConfigureLogContext(provider);

            var context = kernel.Resolve<IMediatorRegistrationContext>();

            return Bus.Factory.CreateMediator(cfg =>
            {
                _configure?.Invoke(context, cfg);
                cfg.ConfigureConsumers(context);
                cfg.ConfigureSagas(context);
            });
        }
    }
}
