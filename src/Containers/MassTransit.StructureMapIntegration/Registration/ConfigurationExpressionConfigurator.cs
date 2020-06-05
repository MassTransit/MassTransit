namespace MassTransit.StructureMapIntegration.Registration
{
    using System;
    using MassTransit.Registration;
    using Monitoring.Health;
    using ScopeProviders;
    using Scoping;
    using StructureMap;
    using Transports;


    public class ConfigurationExpressionConfigurator :
        RegistrationConfigurator,
        IConfigurationExpressionConfigurator
    {
        readonly ConfigurationExpression _expression;

        public ConfigurationExpressionConfigurator(ConfigurationExpression expression)
            : base(new StructureMapContainerRegistrar(expression))
        {
            _expression = expression;

            expression.For<IBusDepot>()
                .Use<BusDepot>()
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

            _expression.For<IBusInstance>()
                .Use<DefaultBusInstance>()
                .Singleton();

            expression.For<IConsumerScopeProvider>()
                .Use(context => CreateConsumerScopeProvider(context))
                .Singleton();

            expression.For<IConfigurationServiceProvider>()
                .Use(context => new StructureMapConfigurationServiceProvider(context.GetInstance<IContainer>()));

            expression.For<IRegistration>()
                .Use(provider => CreateRegistration(provider.GetInstance<IConfigurationServiceProvider>()))
                .Singleton();
        }

        ConfigurationExpression IConfigurationExpressionConfigurator.Builder => _expression;

        public void AddBus(Func<IRegistrationContext<IContext>, IBusControl> busFactory)
        {
            if (busFactory == null)
                throw new ArgumentNullException(nameof(busFactory));

            ThrowIfAlreadyConfigured(nameof(AddBus));

            _expression.For<IBusControl>()
                .Use(context => BusFactory(context, busFactory))
                .Singleton();

            _expression.For<IBus>()
                .Use(context => context.GetInstance<IBusControl>())
                .Singleton();
        }

        public void SetBusFactory<T>(T busFactory)
            where T : IRegistrationBusFactory<IContext>
        {
            throw new NotImplementedException();
        }

        public void AddRider(Action<IRiderConfigurator<IContext>> configure)
        {
            throw new NotImplementedException();
        }

        IBusControl BusFactory(IContext context, Func<IRegistrationContext<IContext>, IBusControl> busFactory)
        {
            var provider = context.GetInstance<IConfigurationServiceProvider>();

            ConfigureLogContext(provider);

            IRegistrationContext<IContext> registrationContext = GetRegistrationContext(context);

            return busFactory(registrationContext);
        }

        static IConsumerScopeProvider CreateConsumerScopeProvider(IContext context)
        {
            return new StructureMapConsumerScopeProvider(context.GetInstance<IContainer>());
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
