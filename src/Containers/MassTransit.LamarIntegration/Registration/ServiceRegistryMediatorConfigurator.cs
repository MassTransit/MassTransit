namespace MassTransit.LamarIntegration.Registration
{
    using System;
    using Lamar;
    using MassTransit.Registration;
    using Mediator;
    using ScopeProviders;
    using Scoping;


    public class ServiceRegistryMediatorConfigurator :
        RegistrationConfigurator,
        IServiceRegistryMediatorConfigurator
    {
        readonly ServiceRegistry _registry;
        Action<IServiceContext, IReceiveEndpointConfigurator> _configure;

        public ServiceRegistryMediatorConfigurator(ServiceRegistry registry)
            : base(new LamarContainerMediatorRegistrar(registry))
        {
            _registry = registry;

            registry.For<IConsumerScopeProvider>()
                .Use(CreateConsumerScopeProvider)
                .Singleton();

            registry.For<IConfigurationServiceProvider>()
                .Use(context => new LamarConfigurationServiceProvider(context.GetInstance<IContainer>()))
                .Singleton();

            registry.Injectable<ConsumeContext>();

            _registry.For<IMediator>()
                .Use(MediatorFactory)
                .Singleton();
        }

        ServiceRegistry IServiceRegistryMediatorConfigurator.Builder => _registry;

        IMediator MediatorFactory(IServiceContext context)
        {
            var provider = context.GetInstance<IConfigurationServiceProvider>();

            ConfigureLogContext(provider);

            return Bus.Factory.CreateMediator(cfg =>
            {
                _configure?.Invoke(context, cfg);

                ConfigureMediator(cfg, provider);
            });
        }

        static IConsumerScopeProvider CreateConsumerScopeProvider(IServiceContext context)
        {
            return new LamarConsumerScopeProvider(context.GetInstance<IContainer>());
        }

        public void ConfigureMediator(Action<IServiceContext, IReceiveEndpointConfigurator> configure)
        {
            _configure = configure;
        }
    }
}
