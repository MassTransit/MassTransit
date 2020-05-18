namespace MassTransit.StructureMapIntegration.Registration
{
    using System;
    using MassTransit.Registration;
    using Mediator;
    using Monitoring.Health;
    using ScopeProviders;
    using Scoping;
    using StructureMap;
    using Transports;


    public class ConfigurationExpressionRegistrationConfigurator :
        RegistrationConfigurator,
        IConfigurationExpressionConfigurator
    {
        readonly ConfigurationExpression _expression;

        public ConfigurationExpressionRegistrationConfigurator(ConfigurationExpression expression)
            : base(new StructureMapContainerRegistrar(expression))
        {
            _expression = expression;

            expression.For<IBusRegistry>()
                .Use<BusRegistry>()
                .Singleton();

            expression.For<IConsumerScopeProvider>()
                .Use(context => CreateConsumerScopeProvider(context))
                .Singleton();

            expression.For<ISagaRepositoryFactory>()
                .Use(context => CreateSagaRepositoryFactory(context))
                .Singleton();

            expression.For<IConfigurationServiceProvider>()
                .Use(context => new StructureMapConfigurationServiceProvider(context.GetInstance<IContainer>()))
                .Singleton();

            expression.For<IRegistrationConfigurator>()
                .Use(this);

            expression.For<IRegistration>()
                .Use(provider => CreateRegistration(provider.GetInstance<IConfigurationServiceProvider>()))
                .Singleton();
        }

        ConfigurationExpression IConfigurationExpressionConfigurator.Builder => _expression;

        public void AddBus(Func<IRegistrationContext<IContext>, IBusControl> busFactory)
        {
            ThrowIfAlreadyConfigured();

            _expression.For<IBusControl>()
                .Use(context => BusFactory(context, busFactory))
                .Singleton();

            _expression.For<IBus>()
                .Use(context => context.GetInstance<IBusControl>())
                .Singleton();

            _expression.For<ISendEndpointProvider>()
                .Use(context => GetCurrentSendEndpointProvider(context))
                .ContainerScoped();

            _expression.For<IPublishEndpoint>()
                .Use(context => GetCurrentPublishEndpoint(context))
                .ContainerScoped();

            _expression.For<IClientFactory>()
                .Use(context => ClientFactoryProvider(context.GetInstance<IConfigurationServiceProvider>(), context.GetInstance<IBus>()))
                .Singleton();

            _expression.For<BusHealth>()
                .Use(context => new BusHealth(nameof(IBus)))
                .Singleton();

            _expression.For<IBusHealth>()
                .Use<BusHealth>()
                .Singleton();

            _expression.For<IBusRegistryInstance>()
                .Use<BusRegistryInstance>()
                .Singleton();
        }

        IBusControl BusFactory(IContext context, Func<IRegistrationContext<IContext>, IBusControl> busFactory)
        {
            var provider = context.GetInstance<IConfigurationServiceProvider>();

            ConfigureLogContext(provider);

            IRegistrationContext<IContext> registrationContext = GetRegistrationContext(context);

            return busFactory(registrationContext);
        }

        public void AddMediator(Action<IContext, IReceiveEndpointConfigurator> configure = null)
        {
            ThrowIfAlreadyConfigured();

            _expression.For<IMediator>()
                .Use(context => MediatorFactory(context, configure))
                .Singleton();

            _expression.For<IClientFactory>()
                .Use(context => context.GetInstance<IMediator>())
                .Singleton();
        }

        IMediator MediatorFactory(IContext context, Action<IContext, IReceiveEndpointConfigurator> configure)
        {
            var provider = context.GetInstance<IConfigurationServiceProvider>();

            ConfigureLogContext(provider);

            return Bus.Factory.CreateMediator(cfg =>
            {
                configure?.Invoke(context, cfg);

                ConfigureMediator(cfg, provider);
            });
        }

        static IConsumerScopeProvider CreateConsumerScopeProvider(IContext context)
        {
            return new StructureMapConsumerScopeProvider(context.GetInstance<IContainer>());
        }

        static ISagaRepositoryFactory CreateSagaRepositoryFactory(IContext context)
        {
            return new StructureMapSagaRepositoryFactory(context.GetInstance<IContainer>());
        }

        static ISendEndpointProvider GetCurrentSendEndpointProvider(IContext context)
        {
            return (ISendEndpointProvider)context.TryGetInstance<ConsumeContext>()
                ?? new ScopedSendEndpointProvider<IContainer>(context.GetInstance<IBus>(), context.GetInstance<IContainer>());
        }

        static IPublishEndpoint GetCurrentPublishEndpoint(IContext context)
        {
            return (IPublishEndpoint)context.TryGetInstance<ConsumeContext>()
                ?? new PublishEndpoint(new ScopedPublishEndpointProvider<IContainer>(context.GetInstance<IBus>(), context.GetInstance<IContainer>()));
        }

        IRegistrationContext<IContext> GetRegistrationContext(IContext context)
        {
            return new RegistrationContext<IContext>(
                CreateRegistration(context.GetInstance<IConfigurationServiceProvider>()),
                context.GetInstance<BusHealth>(),
                context
            );
        }
    }
}
