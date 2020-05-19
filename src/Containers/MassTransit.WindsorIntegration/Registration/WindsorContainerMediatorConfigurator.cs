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
        Action<IKernel, IReceiveEndpointConfigurator> _configure;

        public WindsorContainerMediatorConfigurator(IWindsorContainer container)
            : base(new WindsorContainerMediatorRegistrar(container))
        {
            Container = container;

            container.RegisterScopedContextProviderIfNotPresent();

            if (!container.Kernel.HasComponent(typeof(IConsumerScopeProvider)))
                container.Register(Component.For<IConsumerScopeProvider>()
                    .ImplementedBy<WindsorConsumerScopeProvider>()
                    .LifestyleTransient());

            if (!container.Kernel.HasComponent(typeof(IConfigurationServiceProvider)))
                container.Register(Component.For<IConfigurationServiceProvider>()
                    .ImplementedBy<WindsorConfigurationServiceProvider>()
                    .LifestyleSingleton());

            container.Register(
                Component.For<IMediator>()
                    .UsingFactoryMethod(MediatorFactory)
                    .LifestyleSingleton()
            );
        }

        public IWindsorContainer Container { get; }

        public void ConfigureMediator(Action<IKernel, IReceiveEndpointConfigurator> configure)
        {
            ThrowIfAlreadyConfigured();
            _configure = configure;
        }

        IMediator MediatorFactory(IKernel kernel)
        {
            var provider = kernel.Resolve<IConfigurationServiceProvider>();

            ConfigureLogContext(provider);

            return Bus.Factory.CreateMediator(cfg =>
            {
                _configure?.Invoke(kernel, cfg);

                ConfigureMediator(cfg, provider);
            });
        }
    }
}
